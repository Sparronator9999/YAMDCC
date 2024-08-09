using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;

namespace YAMDCC.IPC
{
    /// <summary>
    /// Wraps a <see cref="NamedPipeServerStream"/> and provides
    /// multiple simultaneous client connection handling.
    /// </summary>
    /// <typeparam name="TRdWr">
    /// Reference type to read from and write to the named pipe
    /// </typeparam>
    public class NamedPipeServer<TRdWr> : Server<TRdWr, TRdWr>
        where TRdWr : class
    {
        /// <summary>
        /// Constructs a new <see cref="NamedPipeServer{TRdWr}"/> object
        /// that listens for client connections on the given <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the pipe to listen on.
        /// </param>
        public NamedPipeServer(string pipeName) : base(pipeName) { }

        /// <summary>
        /// Constructs a new <see cref="NamedPipeServer{TRdWr}"/> object
        /// that listens for client connections on the given <paramref name="pipeName"/>,
        /// with the given <paramref name="security"/>.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the pipe to listen on.
        /// </param>
        /// <param name="bufferSize">
        /// The size of input and output buffer.
        /// </param>
        /// <param name="security">
        /// The object that determines the access control
        /// and audit security for the pipe.
        /// </param>
        public NamedPipeServer(string pipeName, int bufferSize, PipeSecurity security)
            : base(pipeName, bufferSize, security) { }
    }

    /// <summary>
    /// Wraps a <see cref="NamedPipeServerStream"/> and provides
    /// multiple simultaneous client connection handling.
    /// </summary>
    /// <typeparam name="TRd">Reference type to read from the named pipe</typeparam>
    /// <typeparam name="TWr">Reference type to write to the named pipe</typeparam>
    public class Server<TRd, TWr>
        where TRd : class
        where TWr : class
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        public event ConnectionEventHandler<TRd, TWr> ClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        public event ConnectionEventHandler<TRd, TWr> ClientDisconnected;

        /// <summary>
        /// Invoked whenever a client sends a message to the server.
        /// </summary>
        public event ConnectionMessageEventHandler<TRd, TWr> ClientMessage;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation.
        /// </summary>
        public event PipeExceptionEventHandler Error;

        private readonly string _pipeName;
        private readonly int _bufferSize;
        private readonly PipeSecurity _security;
        private readonly List<NamedPipeConnection<TRd, TWr>> _connections = new List<NamedPipeConnection<TRd, TWr>>();

        private int _nextPipeId;

        private volatile bool _shouldKeepRunning;

        /// <summary>
        /// Constructs a new <c>NamedPipeServer</c> object that listens for client connections on the given <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to listen on</param>
        public Server(string pipeName)
        {
            _pipeName = pipeName;
        }

        /// <summary>
        /// Constructs a new <c>NamedPipeServer</c> object that listens
        /// for client connections on the given <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to listen on</param>
        /// <param name="bufferSize">Size of input and output buffer</param>
        /// <param name="security">And object that determine the access control and audit security for the pipe</param>
        public Server(string pipeName, int bufferSize, PipeSecurity security)
        {
            _pipeName = pipeName;
            _bufferSize = bufferSize;
            _security = security;
        }

        /// <summary>
        /// Begins listening for client connections in a separate background thread.
        /// This method returns immediately.
        /// </summary>
        public void Start()
        {
            _shouldKeepRunning = true;
            Worker worker = new Worker();
            worker.Error += OnError;
            worker.DoWork(ListenSync);
        }

        /// <summary>
        /// Sends a message to all connected clients asynchronously.
        /// This method returns immediately, possibly before the
        /// message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        public void PushMessage(TWr message)
        {
            lock (_connections)
            {
                foreach (NamedPipeConnection<TRd, TWr> client in _connections)
                {
                    client.PushMessage(message);
                }
            }
        }

        /// <summary>
        /// Sends a message to a specific client asynchronously.
        /// This method returns immediately, possibly before the message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetId">Specific client ID to send to.</param>
        public void PushMessage(TWr message, int targetId)
        {
            lock (_connections)
            {
                // Can we speed this up with Linq or does that add overhead?
                foreach (NamedPipeConnection<TRd, TWr> client in _connections)
                {
                    if (client.ID == targetId)
                    {
                        client.PushMessage(message);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to a specific clients asynchronously.
        /// This method returns immediately, possibly before the message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetIds">A list of client ID's to send to.</param>
        public void PushMessage(TWr message, List<int> targetIds)
        {
            lock (_connections)
            {
                // Can we speed this up with Linq or does that add overhead?
                foreach (NamedPipeConnection<TRd, TWr> client in _connections)
                {
                    if (targetIds.Contains(client.ID))
                    {
                        client.PushMessage(message);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to a specific clients asynchronously.
        /// This method returns immediately, possibly before the message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetIds">An array of client ID's to send to.</param>
        public void PushMessage(TWr message, int[] targetIds)
        {
            PushMessage(message, targetIds.ToList());
        }

        /// <summary>
        /// Sends a message to a specific client asynchronously.
        /// This method returns immediately, possibly before the message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetName">Specific client name to send to.</param>
        public void PushMessage(TWr message, string targetName)
        {
            lock (_connections)
            {
                // Can we speed this up with Linq or does that add overhead?
                foreach (NamedPipeConnection<TRd, TWr> client in _connections)
                {
                    if (client.Name.Equals(targetName))
                    {
                        client.PushMessage(message);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to a specific client asynchronously.
        /// This method returns immediately, possibly before the message has been sent to all clients.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetNames">A list of client names to send to.</param>
        public void PushMessage(TWr message, List<string> targetNames)
        {
            lock (_connections)
            {
                foreach (NamedPipeConnection<TRd, TWr> client in _connections)
                {
                    if (targetNames.Contains(client.Name))
                    {
                        client.PushMessage(message);
                    }
                }
            }
        }

        /// <summary>
        /// Closes all open client connections and stops listening for new ones.
        /// </summary>
        public void Stop()
        {
            _shouldKeepRunning = false;

            lock (_connections)
            {
                foreach (NamedPipeConnection<TRd, TWr> client in _connections.ToArray())
                {
                    client.Close();
                }
            }

            // If background thread is still listening for a client to connect,
            // initiate a dummy connection that will allow the thread to exit.
            NamedPipeClient<TRd, TWr> dummyClient = new NamedPipeClient<TRd, TWr>(_pipeName);
            dummyClient.Start();
            dummyClient.WaitForConnection(TimeSpan.FromSeconds(2));
            dummyClient.Stop();
            dummyClient.WaitForDisconnection(TimeSpan.FromSeconds(2));
        }

        #region Private methods

        private void ListenSync()
        {
            while (_shouldKeepRunning)
            {
                WaitForConnection();
            }
        }

        private void WaitForConnection()
        {
            NamedPipeServerStream handshakePipe = null;
            NamedPipeServerStream dataPipe = null;
            NamedPipeConnection<TRd, TWr> connection = null;

            string connectionPipeName = GetNextConnectionPipeName();

            try
            {
                dataPipe = CreatePipe(connectionPipeName);

                // Send the client the name of the data pipe to use
                handshakePipe = CreateAndConnectPipe();

                PipeStreamWrapper<string, string> handshakeWrapper
                    = new PipeStreamWrapper<string, string>(handshakePipe);

                handshakeWrapper.WriteObject(connectionPipeName);
                handshakeWrapper.WaitForPipeDrain();
                handshakeWrapper.Close();

                // Wait for the client to connect to the data pipe
                dataPipe.WaitForConnection();

                // Add the client's connection to the list of connections
                connection = ConnectionFactory.CreateConnection<TRd, TWr>(dataPipe);
                connection.ReceiveMessage += ClientOnReceiveMessage;
                connection.Disconnected += ClientOnDisconnected;
                connection.Error += ConnectionOnError;
                connection.Open();

                lock (_connections)
                {
                    _connections.Add(connection);
                }

                ClientOnConnected(connection);
            }
            // Catch the IOException that is raised if the pipe is broken or disconnected.
            catch (Exception e)
            {
                Console.Error.WriteLine("Named pipe is broken or disconnected: {0}", e);

                Cleanup(handshakePipe);
                Cleanup(dataPipe);

                ClientOnDisconnected(connection);
            }
        }

        private NamedPipeServerStream CreateAndConnectPipe()
        {
            return _security == null
                ? PipeServerFactory.CreateAndConnectPipe(_pipeName)
                : PipeServerFactory.CreateAndConnectPipe(_pipeName, _bufferSize, _security);
        }

        private NamedPipeServerStream CreatePipe(string connectionPipeName)
        {
            return _security == null
                ? PipeServerFactory.CreatePipe(connectionPipeName)
                : PipeServerFactory.CreatePipe(connectionPipeName, _bufferSize, _security);
        }

        private void ClientOnConnected(NamedPipeConnection<TRd, TWr> connection) =>
            ClientConnected?.Invoke(connection);

        private void ClientOnReceiveMessage(NamedPipeConnection<TRd, TWr> connection, TRd message) =>
            ClientMessage?.Invoke(connection, message);

        private void ClientOnDisconnected(NamedPipeConnection<TRd, TWr> connection)
        {
            if (connection == null) return;

            lock (_connections)
            {
                _connections.Remove(connection);
            }

            ClientDisconnected?.Invoke(connection);
        }

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

        private string GetNextConnectionPipeName() =>
            $"{_pipeName}_{++_nextPipeId}";

        private static void Cleanup(NamedPipeServerStream pipe)
        {
            if (pipe == null) return;

            using (NamedPipeServerStream x = pipe)
            {
                x.Close();
            }
        }

        #endregion
    }
}
