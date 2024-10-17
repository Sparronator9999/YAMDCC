using MessagePack;
using System;
using System.IO;
using System.IO.Pipes;
using System.Net;

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
            if (obj is not null)
            {
                byte[] data = MessagePackSerializer.Serialize(obj);
                byte[] lenBuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));

                BaseStream.Write(lenBuf, 0, lenBuf.Length);
                BaseStream.Write(data, 0, data.Length);
                BaseStream.Flush();
            }
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
    }
}
