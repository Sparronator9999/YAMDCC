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
    /// Wraps a <see cref="PipeStream"/> object and writes to it.
    /// </summary>
    /// <remarks>
    /// Serializes .NET CLR objects specified by <typeparamref name="T"/>
    /// into binary form and sends them over the named pipe for a
    /// <see cref="PipeStreamWriter{T}"/> to read and deserialize.
    /// </remarks>
    /// <typeparam name="T">
    /// The reference type to serialize.
    /// </typeparam>
    internal sealed class PipeStreamWriter<T> where T : class
    {
        /// <summary>
        /// Gets the underlying <see cref="PipeStream"/> object.
        /// </summary>
        internal PipeStream BaseStream { get; private set; }

        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// Constructs a new <see cref="PipeStreamWriter{T}"/>
        /// object that writes to given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The named pipe to write to.
        /// </param>
        internal PipeStreamWriter(PipeStream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// Writes an object to the pipe.
        /// </summary>
        /// <remarks>
        /// This method blocks until all data is sent.
        /// </remarks>
        /// <param name="obj">
        /// The object to write to the pipe.
        /// </param>
        /// <exception cref="SerializationException"/>
        internal void WriteObject(T obj)
        {
            byte[] data;
            if (typeof(T) == typeof(string))
            {
                data = Encoding.Unicode.GetBytes(obj.ToString());
            }
            else
            {
                data = Serialize(obj);
                WriteLength(data.Length);
            }
            BaseStream.Write(data, 0, data.Length);
            BaseStream.Flush();
        }

        /// <summary>
        /// Waits for the other end of the pipe to read all sent bytes.
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="IOException"/>
        internal void WaitForPipeDrain()
        {
            BaseStream.WaitForPipeDrain();
        }

        /// <exception cref="SerializationException"/>
        private byte[] Serialize(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                _binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        private void WriteLength(int len)
        {
            byte[] lenbuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len));
            BaseStream.Write(lenbuf, 0, lenbuf.Length);
        }
    }
}
