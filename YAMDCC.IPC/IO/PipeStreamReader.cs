using MessagePack;
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;

namespace YAMDCC.IPC.IO
{
    /// <summary>
    /// Wraps a <see cref="PipeStream"/> object and reads from it.
    /// </summary>
    /// <remarks>
    /// Deserializes binary data sent by a <see cref="PipeStreamWriter{T}"/>
    /// into a .NET CLR object specified by <typeparamref name="T"/>.
    /// </remarks>
    /// <typeparam name="T">
    /// The reference type to deserialize data to.
    /// </typeparam>
    internal sealed class PipeStreamReader<T> where T : class
    {
        /// <summary>
        /// Gets the underlying <see cref="PipeStream"/> object.
        /// </summary>
        internal PipeStream BaseStream { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        internal bool IsConnected => BaseStream.IsConnected;

        private readonly MessagePackSerializerOptions _options =
            MessagePackSerializerOptions.Standard.WithSecurity(MessagePackSecurity.UntrustedData);

        private const int SIZE_INT = sizeof(int);

        /// <summary>
        /// Constructs a new <see cref="PipeStreamReader{T}"/> object
        /// that reads data from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The pipe stream to read from.
        /// </param>
        internal PipeStreamReader(PipeStream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// Reads the next object from the pipe.
        /// </summary>
        /// <remarks>
        /// This method blocks until an object is
        /// sent or the pipe is disconnected.
        /// </remarks>
        /// <returns>
        /// The next object read from the pipe, or
        /// <c>null</c> if the pipe disconnected.
        /// </returns>
        /// <exception cref="MessagePackSerializationException"/>
        internal T ReadObject()
        {
            int len = ReadLength();
            return len == 0 ? default : ReadObject(len);
        }

        /// <summary>
        /// Reads the length of the next message (in bytes) from the client.
        /// </summary>
        /// <returns>Number of bytes of data the client will be sending.</returns>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="IOException"/>
        private int ReadLength()
        {
            byte[] lenbuf = new byte[SIZE_INT];
            int bytesRead = BaseStream.Read(lenbuf, 0, SIZE_INT);
            return bytesRead == 0
                ? 0
                : bytesRead != SIZE_INT
                    ? throw new IOException($"Expected {SIZE_INT} bytes, but read {bytesRead}.")
                    : IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenbuf, 0));
        }

        /// <exception cref="MessagePackSerializationException"/>
        private T ReadObject(int len)
        {
            byte[] data = new byte[len];
            int bytesRead = BaseStream.Read(data, 0, data.Length);
            return bytesRead == len
                ? MessagePackSerializer.Deserialize<T>(data, _options)
                : throw new IOException($"Expected {SIZE_INT} bytes, but read {bytesRead}.");

        }
    }
}
