using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Represents a connection between a named pipe client and server.
    /// </summary>
    /// <typeparam name="TRd">Reference type to read from the named pipe</typeparam>
    /// <typeparam name="TWr">Reference type to write to the named pipe</typeparam>
    public sealed class NamedPipeConnection<TRd, TWr>
        where TRd : class
        where TWr : class
    {
        /// <summary>
        /// Gets the connection's unique identifier.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// Gets the connection's name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets the connection's handle.
        /// </summary>
        public readonly SafeHandle Handle;

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected => _streamWrapper.IsConnected;

        /// <summary>
        /// Invoked when the named pipe connection terminates.
        /// </summary>
        public event ConnectionEventHandler<TRd, TWr> Disconnected;

        /// <summary>
        /// Invoked whenever a message is received from the other end of the pipe.
        /// </summary>
        public event ConnectionMessageEventHandler<TRd, TWr> ReceiveMessage;

        /// <summary>
        /// Invoked when an exception is thrown during any read/write operation over the named pipe.
        /// </summary>
        public event ConnectionExceptionEventHandler<TRd, TWr> Error;

        private readonly PipeStreamWrapper<TRd, TWr> _streamWrapper;

        private readonly AutoResetEvent _writeSignal = new AutoResetEvent(false);
        private readonly Queue<TWr> _writeQueue = new Queue<TWr>();

        private bool _notifiedSucceeded;

        internal NamedPipeConnection(int id, string name, PipeStream serverStream)
        {
            ID = id;
            Name = name;
            Handle = serverStream.SafePipeHandle;
            _streamWrapper = new PipeStreamWrapper<TRd, TWr>(serverStream);
        }

        /// <summary>
        /// Begins reading from and writing to the named pipe on a background thread.
        /// This method returns immediately.
        /// </summary>
        public void Open()
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
        /// Adds the specified <paramref name="message"/> to the write queue.
        /// The message will be written to the named pipe by the background thread
        /// at the next available opportunity.
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(TWr message)
        {
            _writeQueue.Enqueue(message);
            _writeSignal.Set();
        }

        /// <summary>
        /// Closes the named pipe connection and underlying <c>PipeStream</c>.
        /// </summary>
        public void Close()
        {
            _streamWrapper.Close();
            _writeSignal.Set();
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        private void OnSucceeded()
        {
            // Only notify observers once
            if (_notifiedSucceeded)
            {
                return;
            }

            _notifiedSucceeded = true;

            Disconnected?.Invoke(this);
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        /// <param name="exception"></param>
        private void OnError(Exception exception) =>
            Error?.Invoke(this, exception);

        /// <summary>
        /// Invoked on the background thread.
        /// </summary>
        /// <exception cref="SerializationException">An object in the graph of type parameter <typeparamref name="TRd"/> is not marked as serializable.</exception>
        private void ReadPipe()
        {
            while (IsConnected && _streamWrapper.CanRead)
            {
                TRd obj = _streamWrapper.ReadObject();
                if (obj == null)
                {
                    Close();
                    return;
                }
                ReceiveMessage?.Invoke(this, obj);
            }
        }

        /// <summary>
        /// Invoked on the background thread.
        /// </summary>
        /// <exception cref="SerializationException">An object in the graph of type parameter <typeparamref name="TWr"/> is not marked as serializable.</exception>
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
    }

    internal static class ConnectionFactory
    {
        private static int _lastId;

        public static NamedPipeConnection<TRd, TWr> CreateConnection<TRd, TWr>(PipeStream pipeStream)
            where TRd : class
            where TWr : class
        {
            return new NamedPipeConnection<TRd, TWr>(++_lastId, "Client " + _lastId, pipeStream);
        }
    }

    /// <summary>
    /// Handles new connections.
    /// </summary>
    /// <param name="connection">The newly established connection</param>
    /// <typeparam name="TRd">Reference type</typeparam>
    /// <typeparam name="TWr">Reference type</typeparam>
    public delegate void ConnectionEventHandler<TRd, TWr>(NamedPipeConnection<TRd, TWr> connection)
        where TRd : class
        where TWr : class;

    /// <summary>
    /// Handles messages received from a named pipe.
    /// </summary>
    /// <typeparam name="TRd">Reference type</typeparam>
    /// <typeparam name="TWr">Reference type</typeparam>
    /// <param name="connection">Connection that received the message</param>
    /// <param name="message">Message sent by the other end of the pipe</param>
    public delegate void ConnectionMessageEventHandler<TRd, TWr>(NamedPipeConnection<TRd, TWr> connection, TRd message)
        where TRd : class
        where TWr : class;

    /// <summary>
    /// Handles exceptions thrown during read/write operations.
    /// </summary>
    /// <typeparam name="TRd">Reference type</typeparam>
    /// <typeparam name="TWr">Reference type</typeparam>
    /// <param name="connection">Connection that threw the exception</param>
    /// <param name="exception">The exception that was thrown</param>
    public delegate void ConnectionExceptionEventHandler<TRd, TWr>(NamedPipeConnection<TRd, TWr> connection, Exception exception)
        where TRd : class
        where TWr : class;
}
