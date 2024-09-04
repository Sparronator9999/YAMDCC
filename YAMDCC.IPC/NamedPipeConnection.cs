using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Represents a connection between a named pipe client and server.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type to read from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type to write to the named pipe.
    /// </typeparam>
    public class NamedPipeConnection<TRead, TWrite> : IDisposable
        where TRead : class
        where TWrite : class
    {
        /// <summary>
        /// Gets the connection's unique identifier.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the connection's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the connection's handle.
        /// </summary>
        public SafeHandle Handle => _streamWrapper.BaseStream.SafePipeHandle;

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected => _streamWrapper.IsConnected;

        /// <summary>
        /// Invoked when the named pipe connection terminates.
        /// </summary>
        public event EventHandler<PipeConnectionEventArgs<TRead, TWrite>> Disconnected;

        /// <summary>
        /// Invoked whenever a message is received from the other end of the pipe.
        /// </summary>
        public event EventHandler<PipeMessageEventArgs<TRead, TWrite>> ReceiveMessage;

        /// <summary>
        /// Invoked when an exception is thrown during any read/write operation over the named pipe.
        /// </summary>
        public event EventHandler<PipeErrorEventArgs<TRead, TWrite>> Error;

        private readonly PipeStreamWrapper<TRead, TWrite> _streamWrapper;

        private readonly AutoResetEvent _writeSignal = new AutoResetEvent(false);
        private readonly Queue<TWrite> _writeQueue = new Queue<TWrite>();

        private bool _notifiedSucceeded;

        private bool _disposed;

        internal NamedPipeConnection(int id, string name, PipeStream serverStream)
        {
            ID = id;
            Name = name;
            _streamWrapper = new PipeStreamWrapper<TRead, TWrite>(serverStream);
        }

        /// <summary>
        /// Adds the specified message to the write queue.
        /// </summary>
        /// <remarks>
        /// The message will be written to the named pipe by the
        /// background thread at the next available opportunity.
        /// </remarks>
        /// <param name="message">
        /// The message to write to the named pipe.
        /// </param>
        public void PushMessage(TWrite message)
        {
            _writeQueue.Enqueue(message);
            _writeSignal.Set();
        }

        /// <summary>
        /// Begins reading from and writing to the
        /// named pipe on a background thread.
        /// </summary>
        /// <remarks>
        /// This method returns immediately.
        /// </remarks>
        internal void Open()
        {
            Worker readWorker = new Worker();
            readWorker.Succeeded += OnSucceeded;
            readWorker.Error += OnError;
            readWorker.DoWork(ReadPipe);

            Worker writeWorker = new Worker();
            writeWorker.Succeeded += OnSucceeded;
            writeWorker.Error += OnError;
            writeWorker.DoWork(WritePipe);
        }

        /// <summary>
        /// Closes the named pipe connection and
        /// underlying <see cref="PipeStream"/>.
        /// </summary>
        /// <remarks>
        /// Invoked on the background thread.
        /// </remarks>
        internal void Close()
        {
            _streamWrapper.Close();
            _writeSignal.Set();
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        private void OnSucceeded(object sender, EventArgs e)
        {
            // Only notify observers once
            if (_notifiedSucceeded)
                return;

            _notifiedSucceeded = true;

            PipeConnectionEventArgs<TRead, TWrite> e2 = new PipeConnectionEventArgs<TRead, TWrite>(this);
            Disconnected?.Invoke(sender, e2);
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        /// <param name="exception"></param>
        private void OnError(object sender, WorkerErrorEventArgs e)
        {
            Error?.Invoke(sender, new PipeErrorEventArgs<TRead, TWrite>(this, e.Exception));
        }

        /// <summary>
        /// Invoked on the background thread.
        /// </summary>
        /// <exception cref="SerializationException"/>
        private void ReadPipe()
        {
            while (IsConnected && _streamWrapper.CanRead)
            {
                TRead obj = _streamWrapper.ReadObject();
                if (obj == null)
                {
                    Close();
                    return;
                }
                PipeMessageEventArgs<TRead, TWrite> e =
                    new PipeMessageEventArgs<TRead, TWrite>(this, obj);

                ReceiveMessage?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Invoked on the background thread.
        /// </summary>
        /// <exception cref="SerializationException"/>
        private void WritePipe()
        {
            while (IsConnected && _streamWrapper.CanWrite)
            {
                _writeSignal.WaitOne();
                while (_writeQueue.Count > 0)
                {
                    _streamWrapper.WriteObject(_writeQueue.Dequeue());
                    _streamWrapper.WaitForPipeDrain();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _writeSignal.Dispose();
            }

            _disposed = true;
        }
    }
}
