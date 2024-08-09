using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Wraps a <see cref="NamedPipeClientStream"/>.
    /// </summary>
    /// <typeparam name="TRdWr">
    /// Reference type to read from and write to the named pipe
    /// </typeparam>
    public class NamedPipeClient<TRdWr> : NamedPipeClient<TRdWr, TRdWr> where TRdWr : class
    {
        /// <summary>
        /// Constructs a new <c>NamedPipeClient</c> to connect to the
        /// <see cref="NamedPipeServer{TReadWrite}"/> specified by
        /// <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        public NamedPipeClient(string pipeName) : base(pipeName) { }
    }

    /// <summary>
    /// Wraps a <see cref="NamedPipeClientStream"/>.
    /// </summary>
    /// <typeparam name="TRd">Reference type to read from the named pipe</typeparam>
    /// <typeparam name="TWr">Reference type to write to the named pipe</typeparam>
    public class NamedPipeClient<TRd, TWr>
        where TRd : class
        where TWr : class
    {
        /// <summary>
        /// Gets or sets whether the client should attempt to reconnect when the pipe breaks
        /// due to an error or the other end terminating the connection.
        /// Default value is <c>true</c>.
        /// </summary>
        public bool AutoReconnect;

        /// <summary>
        /// Gets or sets how long the client waits between a reconnection attempt.
        /// Default value is <c>0</c>.
        /// </summary>
        public int AutoReconnectDelay;

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

        /// <summary>
        /// Constructs a new <see cref="NamedPipeClient{TRd, TWr}"/> to connect to the
        /// <see cref="NamedPipeServer{TReadWrite}"/> specified by
        /// <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        public NamedPipeClient(string pipeName)
        {
            _pipeName = pipeName;
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
        public void PushMessage(TWr message) => _connection?.PushMessage(message);

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
            PipeStreamWrapper<string, string> handshake = PipeClientFactory.Connect<string, string>(_pipeName);
            string dataPipeName = handshake.ReadObject();
            handshake.Close();

            // Connect to the actual data pipe
            NamedPipeClientStream dataPipe = PipeClientFactory.CreateAndConnectPipe(dataPipeName);

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
            if (Disconnected != null)
                Disconnected(connection);

            _disconnected.Set();

            // Reconnect
            if (AutoReconnect && !_closedExplicitly)
            {
                Thread.Sleep(AutoReconnectDelay);
                Start();
            }
        }

        private void OnReceiveMessage(NamedPipeConnection<TRd, TWr> connection, TRd message)
        {
            if (ServerMessage != null)
                ServerMessage(connection, message);
        }

        /// <summary>
        ///     Invoked on the UI thread.
        /// </summary>
        private void ConnectionOnError(NamedPipeConnection<TRd, TWr> connection, Exception exception)
        {
            OnError(exception);
        }

        /// <summary>
        ///     Invoked on the UI thread.
        /// </summary>
        /// <param name="exception"></param>
        private void OnError(Exception exception)
        {
            if (Error != null)
                Error(exception);
        }

        #endregion
    }

    internal static class PipeClientFactory
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);
        
        public static bool NamedPipeExists(string pipeName)
        {
            try
            {
                bool exists = WaitNamedPipe(pipeName, -1);
                if (!exists)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error == 0 || error == 2)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static PipeStreamWrapper<TRd, TWr> Connect<TRd, TWr>(string pipeName)
            where TRd : class
            where TWr : class
        {
            return new PipeStreamWrapper<TRd, TWr>(CreateAndConnectPipe(pipeName));
        }

        public static NamedPipeClientStream CreateAndConnectPipe(string pipeName, int timeout = 10)
        {
            string normalizedPath = Path.GetFullPath(string.Format(@"\\.\pipe\{0}", pipeName));
            while (!NamedPipeExists(normalizedPath))
            {
                Thread.Sleep(timeout);
            }
            NamedPipeClientStream pipe = CreatePipe(pipeName);
            pipe.Connect(1000);
            return pipe;
        }

        private static NamedPipeClientStream CreatePipe(string pipeName) =>
            new NamedPipeClientStream(".", pipeName, PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
    }
}
