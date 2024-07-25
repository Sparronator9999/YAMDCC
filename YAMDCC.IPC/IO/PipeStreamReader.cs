using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.Serialization;
using MessagePack;

namespace YAMDCC.IPC.IO
{
    /// <summary>
    /// Wraps a <see cref="PipeStream"/> object and reads from it. Deserializes
    /// binary data sent by a <see cref="PipeStreamWriter{T}"/> into a .NET CLR
    /// object specified by <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Reference type to deserialize data to</typeparam>
    public class PipeStreamReader<T>
    {
        /// <summary>
        /// Gets the underlying <c>PipeStream</c> object.
        /// </summary>
        public PipeStream BaseStream { get; }

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Constructs a new <c>PipeStreamReader</c> object that
        /// reads data from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Pipe to read from</param>
        public PipeStreamReader(PipeStream stream)
        {
            BaseStream = stream;
            IsConnected = stream.IsConnected;
        }

        #region Private stream readers

        /// <summary>
        /// Reads the length of the next message (in bytes) from the client.
        /// </summary>
        /// <returns>Number of bytes of data the client will be sending.</returns>
        /// <exception cref="InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
        /// <exception cref="IOException">Any I/O error occurred.</exception>
        private int ReadLength()
        {
            const int lensize = sizeof(int);
            byte[] lenbuf = new byte[lensize];
            int bytesRead = BaseStream.Read(lenbuf, 0, lensize);
            if (bytesRead == 0)
            {
                IsConnected = false;
                return 0;
            }
            return bytesRead != lensize
                ? throw new IOException($"Expected {lensize} bytes but read {bytesRead}")
                : IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenbuf, 0));
        }

        private T ReadObject(int len)
        {
            byte[] data = new byte[len];
            BaseStream.Read(data, 0, len);

            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                return MessagePackSerializer.Deserialize<T>(memoryStream);
            }
        }

        #endregion

        /// <summary>
        /// Reads the next object from the pipe. This method blocks until an
        /// object is sent or the pipe is disconnected.
        /// </summary>
        /// <returns>
        /// The next object read from the pipe, or
        /// <c>null</c> if the pipe disconnected.
        /// </returns>
        /// <exception cref="SerializationException">
        /// An object in the graph of type parameter
        /// <typeparamref name="T"/> is not marked as serializable.
        /// </exception>
        public T ReadObject()
        {
            int len = ReadLength();
            return len == 0 ? default : ReadObject(len);
        }
    }
}