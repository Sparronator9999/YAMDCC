using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;

namespace MSIFanControl.IPC.IO
{
	/// <summary>
	/// Wraps a <see cref="PipeStream"/> object to read and write .NET CLR objects.
	/// </summary>
	/// <typeparam name="TRdWr">Reference type to read from and write to the pipe</typeparam>
	public class PipeStreamWrapper<TRdWr> : PipeStreamWrapper<TRdWr, TRdWr>
		where TRdWr : class
	{
		/// <summary>
		/// Constructs a new <c>PipeStreamWrapper</c> object that reads from and writes to the given <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">Stream to read from and write to</param>
		public PipeStreamWrapper(PipeStream stream) : base(stream) { }
	}

	/// <summary>
	/// Wraps a <see cref="PipeStream"/> object to read and write .NET CLR objects.
	/// </summary>
	/// <typeparam name="TRd">Reference type to <b>read</b> from the pipe</typeparam>
	/// <typeparam name="TWr">Reference type to <b>write</b> to the pipe</typeparam>
	public class PipeStreamWrapper<TRd, TWr>
	{
		/// <summary>
		/// Gets the underlying <c>PipeStream</c> object.
		/// </summary>
		public readonly PipeStream BaseStream;

		/// <summary>
		/// Gets a value indicating whether the
		/// <see cref="BaseStream"/> object is connected or not.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the <see cref="BaseStream"/>
		/// object is connected; otherwise, <c>false</c>.
		/// </returns>
		public bool IsConnected => BaseStream.IsConnected && _reader.IsConnected;

		/// <summary>
		/// Gets a value indicating whether the
		/// current stream supports read operations.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the stream supports read
		/// operations; otherwise, <c>false</c>.
		/// </returns>
		public bool CanRead => BaseStream.CanRead;

		/// <summary>
		/// Gets a value indicating whether the
		/// current stream supports write operations.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the stream supports write
		/// operations; otherwise, <c>false</c>.
		/// </returns>
		public bool CanWrite => BaseStream.CanWrite;

		private readonly PipeStreamReader<TRd> _reader;
		private readonly PipeStreamWriter<TWr> _writer;

		/// <summary>
		/// Constructs a new <c>PipeStreamWrapper</c> object that reads from
		/// and writes to the given <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">Stream to read from and write to</param>
		public PipeStreamWrapper(PipeStream stream)
		{
			BaseStream = stream;
			_reader = new PipeStreamReader<TRd>(BaseStream);
			_writer = new PipeStreamWriter<TWr>(BaseStream);
		}

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
		/// <typeparamref name="TRd"/> is not marked as serializable.
		/// </exception>
		public TRd ReadObject() =>
			_reader.ReadObject();

		/// <summary>
		/// Writes an object to the pipe. This
		/// method blocks until all data is sent.
		/// </summary>
		/// <param name="obj">Object to write to the pipe</param>
		/// <exception cref="SerializationException">
		/// An object in the graph of type parameter
		/// <typeparamref name="TRd"/> is not marked as serializable.
		/// </exception>
		public void WriteObject(TWr obj) =>
			_writer.WriteObject(obj);

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
		public void WaitForPipeDrain() =>
			_writer.WaitForPipeDrain();

		/// <summary>
		/// Closes the current stream and releases any resources (such as
		/// sockets and file handles) associated with the current stream.
		/// </summary>
		public void Close() =>
			BaseStream.Close();
	}
}
