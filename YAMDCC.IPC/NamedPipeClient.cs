using System;
using System.IO.Pipes;
using System.Threading;
using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Wraps a <see cref="NamedPipeClientStream"/>.
    /// </summary>
    /// <typeparam name="TReadWrite">
    /// The reference type to read from and write to the named pipe.
    /// </typeparam>
    public class NamedPipeClient<TReadWrite> : NamedPipeClient<TReadWrite, TReadWrite>
        where TReadWrite : class
    {
        /// <summary>
        /// Constructs a new <see cref="NamedPipeClient{TReadWrite}"/> to
        /// connect to the <see cref="NamedPipeServer{TReadWrite}"/> specified
        /// by <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the named pipe server.
        /// </param>
        public NamedPipeClient(string pipeName) : base(pipeName) { }
    }

    /// <summary>
    /// Wraps a <see cref="NamedPipeClientStream"/>.
    /// </summary>
    /// <typeparam name="TRead">
    /// The reference type to read from the named pipe.
    /// </typeparam>
    /// <typeparam name="TWrite">
    /// The reference type to write to the named pipe.
    /// </typeparam>
    public class NamedPipeClient<TRead, TWrite> : IDisposable
        where TRead : class
        where TWrite : class
    {
        /// <summary>
        /// Gets or sets whether the client should attempt to reconnect when
        /// the pipe breaks due to an error or the other end terminating the
        /// connection.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        /// Gets or sets how long the client
        /// waits between a reconnection attempt.
        /// </summary>
        /// <remarks>
        /// The default value is <c>0</c>.
        /// </remarks>
        public int AutoReconnectDelay { get; set; }

        /// <summary>
        /// Invoked whenever a message is received from the server.
        /// </summary>
        public event EventHandler<PipeMessageEventArgs<TRead, TWrite>> ServerMessage;

        /// <summary>
        /// Invoked when the client disconnects from the server
        /// (e.g. when the pipe is closed or broken).
        /// </summary>
        public event EventHandler<PipeConnectionEventArgs<TRead, TWrite>> Disconnected;

        /// <summary>
        /// Invoked whenever an exception is thrown during
        /// a read or write operation on the named pipe.
        /// </summary>
        public event EventHandler<PipeErrorEventArgs<TRead, TWrite>> Error;

        private readonly string _pipeName;
        private NamedPipeConnection<TRead, TWrite> _connection;

        private readonly AutoResetEvent _connected = new(false);
        private readonly AutoResetEvent _disconnected = new(false);

        private volatile bool _closedExplicitly;

        private bool _disposed;

        /// <summary>
        /// Constructs a new <see cref="NamedPipeClient{TRead,TWrite}"/> to
        /// connect to the <see cref="NamedPipeServer{TRead,TWrite}"/>
        /// specified by <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the named pipe server.
        /// </param>
        public NamedPipeClient(string pipeName)
        {
            _pipeName = pipeName;
            AutoReconnect = true;
        }

        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// </summary>
        /// <remarks>
        /// This method returns immediately, possibly before the connection
        /// has been established. Use <see cref="WaitForConnection()"/> to
        /// wait until the connection to the server is established.
        /// </remarks>
        public void Start()
        {
            _closedExplicitly = false;
            Worker worker = new();
            worker.Error += WorkerOnError;
            worker.DoWork(ListenSync);
        }

        /// <summary>
        /// Closes the named pipe.
        /// </summary>
        public void Stop()
        {
            _closedExplicitly = true;
            _connection?.Close();
        }

        /// <summary>
        /// Sends a message to the server over a named pipe.
        /// </summary>
        /// <param name="message">
        /// The message to send to the server.
        /// </param>
        public void PushMessage(TWrite message)
        {
            _connection?.PushMessage(message);
        }

        #region Wait for connection/disconnection

        /// <summary>
        /// Blocks the current thread until a connection
        /// to the named pipe server is established.
        /// </summary>
        public bool WaitForConnection()
        {
            return _connected.WaitOne();
        }

        /// <summary>
        /// Blocks the current thread until a connection to the
        /// named pipe server is established, waiting until at
        /// most <paramref name="timeout"/> before returning.
        /// </summary>
        /// <param name="timeout">
        /// The timeout, in milliseconds, to wait for the server connection.
        /// </param>
        /// <returns>
        /// <c>true</c> if the server connection was established
        /// before the timeout, otherwise <c>false</c>.
        /// </returns>
        public bool WaitForConnection(int timeout)
        {
            return _connected.WaitOne(timeout);
        }

        /// <summary>
        /// Blocks the current thread until a connection to the
        /// named pipe server is established, waiting until at
        /// most <paramref name="timeout"/> before returning.
        /// </summary>
        /// <param name="timeout">
        /// A <see cref="TimeSpan"/> representing the time
        /// (in milliseconds) to wait for the server connection.
        /// </param>
        /// <returns>
        /// <c>true</c> if the server connection was established
        /// before the timeout, otherwise <c>false</c>.
        /// </returns>
        public bool WaitForConnection(TimeSpan timeout)
        {
            return _connected.WaitOne(timeout);
        }

        /// <summary>
        /// Blocks the current thread until the client
        /// disconnects from the named pipe server.
        /// </summary>
        public bool WaitForDisconnection()
        {
            return _disconnected.WaitOne();
        }

        /// <summary>
        /// Blocks the current thread until the client disconnects
        /// from the named pipe server, waiting until at most
        /// <paramref name="timeout"/> before returning.
        /// </summary>
        /// <param name="timeout">
        /// The timeout, in milliseconds, to wait for the server to disconnect.
        /// </param>
        /// <returns>
        /// <c>true</c> if the client disconnected
        /// before the timeout, otherwise <c>false</c>.
        /// </returns>
        public bool WaitForDisconnection(int timeout)
        {
            return _disconnected.WaitOne(timeout);
        }

        /// <summary>
        /// Blocks the current thread until the client disconnects
        /// from the named pipe server, waiting until at most
        /// <paramref name="timeout"/> before returning.
        /// </summary>
        /// <param name="timeout">
        /// A <see cref="TimeSpan"/> representing the time
        /// (in milliseconds) to wait for the server to disconnect.
        /// </param>
        /// <returns>
        /// <c>true</c> if the client disconnected
        /// before the timeout, otherwise <c>false</c>.
        /// </returns>
        public bool WaitForDisconnection(TimeSpan timeout)
        {
            return _disconnected.WaitOne(timeout);
        }

        #endregion

        #region Private methods
        private void ListenSync()
        {
            // Get the name of the data pipe that should be used from now on by this NamedPipeClient
            PipeStreamWrapper<string, string> handshake = PipeClientFactory.Connect<string, string>(_pipeName);
            string dataPipeName = handshake.ReadObject();
            handshake.Close();

            // Connect to the actual data pipe
            NamedPipeClientStream dataPipe = PipeClientFactory.CreateAndConnectPipe(dataPipeName);

            // Create a Connection object for the data pipe
            _connection = ConnectionFactory.CreateConnection<TRead, TWrite>(dataPipe);
            _connection.Disconnected += OnDisconnected;
            _connection.ReceiveMessage += OnReceiveMessage;
            _connection.Error += ConnectionOnError;
            _connection.Open();

            _connected.Set();
        }

        private void OnDisconnected(object sender, PipeConnectionEventArgs<TRead, TWrite> e)
        {
            Disconnected?.Invoke(sender, e);

            _disconnected.Set();

            // Reconnect
            if (AutoReconnect && !_closedExplicitly)
            {
                Thread.Sleep(AutoReconnectDelay);
                Start();
            }
        }

        private void OnReceiveMessage(object sender, PipeMessageEventArgs<TRead, TWrite> e)
        {
            ServerMessage?.Invoke(sender, e);
        }

        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        private void ConnectionOnError(object sender, PipeErrorEventArgs<TRead, TWrite> e)
        {
            Error?.Invoke(sender, e);
        }


        /// <summary>
        /// Invoked on the UI thread.
        /// </summary>
        private void WorkerOnError(object sender, WorkerErrorEventArgs e)
        {
            Error?.Invoke(sender, new PipeErrorEventArgs<TRead, TWrite>(_connection, e.Exception));
        }
        #endregion

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
                _connected.Dispose();
                _disconnected.Dispose();
            }

            _disposed = true;
        }
    }
}
