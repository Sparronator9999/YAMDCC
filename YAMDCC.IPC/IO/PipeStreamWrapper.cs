using System;
using System.IO;
using System.IO.Pipes;

namespace YAMDCC.IPC.IO
{
    /// <summary>
    /// Wraps a <see cref="PipeStream"/> object
    /// to read and write .NET CLR objects.
    /// </summary>
    /// <typeparam name="TReadWrite">
    /// The reference type to read from and write to the pipe.
    /// </typeparam>
    internal sealed class PipeStreamWrapper<TReadWrite> : PipeStreamWrapper<TReadWrite, TReadWrite>
        where TReadWrite : class
    {
        /// <summary>
        /// Constructs a new <see cref="PipeStreamWrapper{TReadWrite}"/> object
        /// that reads from and writes to the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The pipe stream to read from and write to.
        /// </param>
        public PipeStreamWrapper(PipeStream stream)
            : base(stream) { }
    }

    /// <summary>
    /// Wraps a <see cref="PipeStream"/> object
    /// to read and write .NET CLR objects.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type to read from the pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type to write to the pipe.
    /// </typeparam>
    internal class PipeStreamWrapper<TRead, TWrite>
        where TRead : class
        where TWrite : class
    {
        /// <summary>
        /// Gets the underlying <see cref="PipeStream"/> object.
        /// </summary>
        internal PipeStream BaseStream { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="BaseStream"/> object is connected or not.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="BaseStream"/>
        /// object is connected, otherwise <c>false</c>.
        /// </returns>
        internal bool IsConnected => BaseStream.IsConnected && _reader.IsConnected;

        /// <summary>
        /// Gets a value indicating whether the
        /// current stream supports read operations.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the stream supports read
        /// operations, otherwise <c>false</c>.
        /// </returns>
        internal bool CanRead => BaseStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current
        /// stream supports write operations.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the stream supports write
        /// operation, otherwise <c>false</c>.
        /// </returns>
        internal bool CanWrite => BaseStream.CanWrite;

        private readonly PipeStreamReader<TRead> _reader;
        private readonly PipeStreamWriter<TWrite> _writer;

        /// <summary>
        /// Constructs a new <see cref="PipeStreamWrapper{TRead, TWrite}"/>
        /// object that reads from and writes to the given
        /// <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The stream to read from and write to.
        /// </param>
        internal PipeStreamWrapper(PipeStream stream)
        {
            BaseStream = stream;
            _reader = new PipeStreamReader<TRead>(BaseStream);
            _writer = new PipeStreamWriter<TWrite>(BaseStream);
        }

        /// <summary>
        /// Reads the next object from the pipe.
        /// </summary>
        /// <remarks>
        /// This method blocks until an object
        /// is sent or the pipe is disconnected.
        /// </remarks>
        /// <returns>
        /// The next object read from the pipe, or
        /// <c>null</c> if the pipe disconnected.
        /// </returns>
        internal TRead ReadObject()
        {
            return _reader.ReadObject();
        }

        /// <summary>
        /// Writes an object to the pipe.
        /// </summary>
        /// <remarks>
        /// This method blocks until all data is sent.
        /// </remarks>
        /// <param name="obj">
        /// Tne object to write to the pipe.
        /// </param>
        internal void WriteObject(TWrite obj)
        {
            _writer.WriteObject(obj);
        }

        /// <summary>
        /// Waits for the other end of the pipe to read all sent bytes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The pipe is closed.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The pipe does not support write operations.
        /// </exception>
        /// <exception cref="IOException">
        /// The pipe is broken or another I/O error occurred.
        /// </exception>
        internal void WaitForPipeDrain()
        {
            _writer.WaitForPipeDrain();
        }

        /// <summary>
        /// Closes the current stream and releases any
        /// resources (such as sockets and file handles)
        /// associated with the current stream.
        /// </summary>
        internal void Close()
        {
            BaseStream.Close();
        }
    }
}
