using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;
using System;
using System.IO.Pipes;
using System.Threading;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Wraps a <see cref="NamedPipeClientStream"/>.
    /// </summary>
    /// <typeparam name="TRdWr">
    /// Reference type to read from and write to the named pipe
    /// </typeparam>
    public class NamedPipeClient<TRdWr> : NamedPipeClient<TRdWr, TRdWr>
    {
        /// <summary>
        /// Constructs a new <c>NamedPipeClient</c> to connect to the
        /// <see cref="NamedPipeServer{TReadWrite}"/> specified by
        /// <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        /// <param name="serverName">server name default is local.</param>
        public NamedPipeClient(string pipeName, string serverName = ".")
            : base(pipeName, serverName) { }
    }

    /// <summary>
    /// Wraps a <see cref="NamedPipeClientStream"/>.
    /// </summary>
    /// <typeparam name="TRd">Reference type to read from the named pipe</typeparam>
    /// <typeparam name="TWr">Reference type to write to the named pipe</typeparam>
    public class NamedPipeClient<TRd, TWr> : IDisposable
    {
        /// <summary>
        /// Gets or sets whether the client should attempt to reconnect when the pipe breaks
        /// due to an error or the other end terminating the connection.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool AutoReconnect;

        /// <summary>
        /// Invoked whenever a message is received from the server.
        /// </summary>
        public event ConnectionMessageEventHandler<TRd, TWr> ServerMessage;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        public event ConnectionEventHandler<TRd, TWr> Disconnected;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        public event PipeExceptionEventHandler Error;

        private readonly string _pipeName;
        private NamedPipeConnection<TRd, TWr> _connection;

        private readonly AutoResetEvent _connected = new AutoResetEvent(false);
        private readonly AutoResetEvent _disconnected = new AutoResetEvent(false);

        private volatile bool _closedExplicitly;
        private bool _disposed;

        /// <summary>
        /// The server name, which client will connect to.
        /// </summary>
        private readonly string _serverName;

        /// <summary>
        /// Constructs a new <c>NamedPipeClient</c> to connect to the named pipe server specified by <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        /// <param name="serverName">the Name of the server, default is  local machine</param>
        public NamedPipeClient(string pipeName, string serverName = ".")
        {
            _pipeName = pipeName;
            _serverName = serverName;
            AutoReconnect = true;
        }

        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// This method returns immediately, possibly before the connection has been established.
        /// </summary>
        public void Start()
        {
            _closedExplicitly = false;
            Worker worker = new Worker();
            worker.Error += OnError;
            worker.DoWork(ListenSync);
        }

        /// <summary>
        /// Sends a message to the server over a named pipe.
        /// </summary>
        /// <param name="message">Message to send to the server.</param>
        public void PushMessage(TWr message) =>
            _connection?.PushMessage(message);

        /// <summary>
        /// Closes the named pipe.
        /// </summary>
        public void Stop()
        {
            _closedExplicitly = true;
            _connection?.Close();
        }

        #region Wait for connection/disconnection
        /// <summary>
        /// Blocks the current thread until a connection is established.
        /// </summary>
        public void WaitForConnection() =>
            _connected.WaitOne();

        /// <summary>
        /// Blocks the current thread until a connection is established,
        /// using an integer to specify the timeout in milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or
        /// <see cref="Timeout.Infinite"/> to wait indefinitely.
        /// </param>
        public void WaitForConnection(int millisecondsTimeout) =>
            _connected.WaitOne(millisecondsTimeout);

        /// <summary>
        /// Blocks the current thread until a connection is established,
        /// using a <see cref="TimeSpan"/> to specify the timeout in milliseconds.
        /// </summary>
        /// <param name="timeout">
        /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait.
        /// </param>
        public void WaitForConnection(TimeSpan timeout) =>
            _connected.WaitOne(timeout);

        /// <summary>
        /// Blocks the current thread until the client disconnects.
        /// </summary>
        public void WaitForDisconnection() =>
            _disconnected.WaitOne();

        /// <summary>
        /// Blocks the current thread until the client disconnects,
        /// using an integer to specify the timeout in milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or
        /// <see cref="Timeout.Infinite"/> to wait indefinitely.
        /// </param>
        public void WaitForDisconnection(int millisecondsTimeout) =>
            _disconnected.WaitOne(millisecondsTimeout);

        /// <summary>
        /// Blocks the current thread until the client disconnects,
        /// using a <see cref="TimeSpan"/> to specify the timeout in milliseconds.
        /// </summary>
        /// <param name="timeout">
        /// A <see cref="TimeSpan"/> that represents the number of milliseconds to wait.
        /// </param>
        public void WaitForDisconnection(TimeSpan timeout) =>
            _disconnected.WaitOne(timeout);

        #endregion

        #region Private methods

        private void ListenSync()
        {
            // Get the name of the data pipe that should be used from now on by this NamedPipeClient
            PipeStreamWrapper<string, string> handshake = PipeClientFactory.Connect<string, string>(_pipeName, _serverName);
            string dataPipeName = handshake.ReadObject();
            handshake.Close();

            // Connect to the actual data pipe
            NamedPipeClientStream dataPipe = PipeClientFactory.CreateAndConnectPipe(dataPipeName, _serverName);

            // Create a Connection object for the data pipe
            _connection = ConnectionFactory.CreateConnection<TRd, TWr>(dataPipe);
            _connection.Disconnected += OnDisconnected;
            _connection.ReceiveMessage += OnReceiveMessage;
            _connection.Error += ConnectionOnError;
            _connection.Open();

            _connected.Set();
        }

        private void OnDisconnected(NamedPipeConnection<TRd, TWr> connection)
        {
            Disconnected?.Invoke(connection);

            _disconnected.Set();

            // Reconnect
            if (AutoReconnect && !_closedExplicitly)
                Start();
        }

        private void OnReceiveMessage(NamedPipeConnection<TRd, TWr> connection, TRd message) =>
            ServerMessage?.Invoke(connection, message);

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        private void ConnectionOnError(NamedPipeConnection<TRd, TWr> connection, Exception exception) =>
            OnError(exception);

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        /// <param name="exception"></param>
        private void OnError(Exception exception) =>
            Error?.Invoke(exception);

        #endregion

        #region Cleanup/Dispose code
        /// <summary>
        /// The destructor for the <see cref="NamedPipeClient{TRead, TWrite}"/>.
        /// </summary>
        ~NamedPipeClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources associated with the
        /// <see cref="NamedPipeClient{TRead, TWrite}"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _connected.Dispose();
                _disconnected.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }

    internal static class PipeClientFactory
    {
        public static PipeStreamWrapper<TRead, TWrite> Connect<TRead, TWrite>(string pipeName, string serverName) =>
            new PipeStreamWrapper<TRead, TWrite>(CreateAndConnectPipe(pipeName, serverName));

        public static NamedPipeClientStream CreateAndConnectPipe(string pipeName, string serverName)
        {
            NamedPipeClientStream pipe = CreatePipe(pipeName, serverName);
            pipe.Connect();
            return pipe;
        }

        private static NamedPipeClientStream CreatePipe(string pipeName, string serverName) =>
            new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
    }
}
