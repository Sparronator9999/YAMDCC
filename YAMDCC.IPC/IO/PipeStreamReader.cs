using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace YAMDCC.IPC.IO
{
    /// <summary>
    /// Wraps a <see cref="PipeStream"/> object and reads from it.
    /// Deserializes binary data sent by a <see cref="PipeStreamWriter{T}"/>
    /// into a .NET CLR object specified by <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Reference type to deserialize data to
    /// </typeparam>
    public class PipeStreamReader<T> where T : class
    {
        /// <summary>
        /// Gets the underlying <c>PipeStream</c> object.
        /// </summary>
        public PipeStream BaseStream { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected { get; private set; }

        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// Constructs a new <see cref="PipeStreamReader{T}"/> object that
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
        /// <returns>
        /// Number of bytes of data the client will be sending.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The pipe is disconnected, waiting to connect,
        /// or the handle has not been set.
        /// </exception>
        /// <exception cref="IOException">
        /// Any I/O error occurred.
        /// </exception>
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

        /// <exception cref="SerializationException">
        /// An object in the graph of type parameter
        /// <typeparamref name="T"/> is not marked as serializable.
        /// </exception>
        private T ReadObject(int len)
        {
            byte[] data = new byte[len];
            BaseStream.Read(data, 0, len);
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                return (T) _binaryFormatter.Deserialize(memoryStream);
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
            if (typeof(T) == typeof(string))
            {
                return (T) ReadString();
            }
            int len = ReadLength();
            return len == 0 ? default : ReadObject(len);
        }

        private object ReadString()
        {
            const int bufferSize = 1024;
            byte[] data = new byte[bufferSize];
            BaseStream.Read(data, 0, bufferSize);
            string message = Encoding.Unicode.GetString(data).TrimEnd('\0');
            
            return message.Length > 0 ? message : null;
        }
    }
}
