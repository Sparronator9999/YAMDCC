using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using YAMDCC.IPC.IO;
using YAMDCC.IPC.Threading;

namespace YAMDCC.IPC;

/// <summary>
/// Wraps a <see cref="NamedPipeServerStream"/> and provides
/// multiple simultaneous client connection handling.
/// </summary>
/// <typeparam name="TReadWrite">
/// The reference type to read from and write to the named pipe.
/// </typeparam>
public class NamedPipeServer<TReadWrite> : NamedPipeServer<TReadWrite, TReadWrite>
    where TReadWrite : class
{
    /// <summary>
    /// Constructs a new <see cref="NamedPipeServer{TReadWrite}"/>
    /// object that listens for client connections on the given
    /// <paramref name="pipeName"/>.
    /// </summary>
    /// <inheritdoc cref="NamedPipeServer{TRead, TWrite}(string)"/>
    public NamedPipeServer(string pipeName)
        : base(pipeName) { }

    /// <summary>
    /// Constructs a new <see cref="NamedPipeServer{TReadWrite}"/>
    /// object that listens for client connections on the given
    /// <paramref name="pipeName"/>.
    /// </summary>
    /// <inheritdoc cref="NamedPipeServer{TRead, TWrite}(string, PipeSecurity, int)"/>
    public NamedPipeServer(string pipeName, PipeSecurity security, int bufferSize = 0)
        : base(pipeName, security, bufferSize) { }
}

/// <summary>
/// Wraps a <see cref="NamedPipeServerStream"/> and provides
/// multiple simultaneous client connection handling.
/// </summary>
/// <typeparam name="TRead">
/// The reference type to read from the named pipe.
/// </typeparam>
/// <typeparam name="TWrite">
/// The reference type to write to the named pipe.
/// </typeparam>
public class NamedPipeServer<TRead, TWrite>
    where TRead : class
    where TWrite : class
{
    /// <summary>
    /// Invoked whenever a client connects to the server.
    /// </summary>
    public event EventHandler<PipeConnectionEventArgs<TRead, TWrite>> ClientConnected;

    /// <summary>
    /// Invoked whenever a client disconnects from the server.
    /// </summary>
    public event EventHandler<PipeConnectionEventArgs<TRead, TWrite>> ClientDisconnected;

    /// <summary>
    /// Invoked whenever a client sends a message to the server.
    /// </summary>
    public event EventHandler<PipeMessageEventArgs<TRead, TWrite>> ClientMessage;

    /// <summary>
    /// Invoked whenever an exception is thrown
    /// during a read or write operation.
    /// </summary>
    public event EventHandler<PipeErrorEventArgs<TRead, TWrite>> Error;

    private readonly string _pipeName;
    private readonly int _bufferSize;
    private readonly PipeSecurity _security;
    private readonly List<NamedPipeConnection<TRead, TWrite>> _connections = [];

    private int _nextPipeId;

    private volatile bool _shouldKeepRunning;

    /// <summary>
    /// Constructs a new <see cref="NamedPipeServer{TRead, TWrite}"/>
    /// object that listens for client connections on the given
    /// <paramref name="pipeName"/>.
    /// </summary>
    /// <param name="pipeName">
    /// The name of the pipe to listen on.
    /// </param>
    public NamedPipeServer(string pipeName)
    {
        _pipeName = pipeName;
    }

    /// <param name="security">
    /// An object that determine the access control
    /// and audit security for the pipe.
    /// </param>
    /// <param name="bufferSize">
    /// <para>The size of the input and output buffer.</para>
    /// <para>Use <c>0</c> for the default buffer size.</para>
    /// </param>
    /// <inheritdoc cref="NamedPipeServer{TRead, TWrite}(string)"/>
    public NamedPipeServer(string pipeName, PipeSecurity security, int bufferSize = 0)
    {
        _pipeName = pipeName;
        _security = security;
        _bufferSize = bufferSize;
    }

    /// <summary>
    /// Begins listening for client connections
    /// in a separate background thread.
    /// </summary>
    /// <remarks>
    /// This method returns immediately.
    /// </remarks>
    public void Start()
    {
        _shouldKeepRunning = true;
        Worker worker = new();
        worker.Error += WorkerOnError;
        worker.DoWork(ListenSync);
    }

    /// <summary>
    /// Sends a message to all connected clients asynchronously.
    /// </summary>
    /// <remarks>
    /// This method returns immediately, possibly before
    /// the message has been sent to all clients.
    /// </remarks>
    /// <param name="message">
    /// The message to send to the clients.
    /// </param>
    public void PushMessage(TWrite message)
    {
        lock (_connections)
        {
            foreach (NamedPipeConnection<TRead, TWrite> client in _connections)
            {
                client.PushMessage(message);
            }
        }
    }

    /// <summary>
    /// Sends a message to a specified client asynchronously.
    /// </summary>
    /// <param name="message">
    /// The message to send to the client.
    /// </param>
    /// <param name="targetId">
    /// The client ID to send the message to.
    /// </param>
    /// <inheritdoc cref="PushMessage(TWrite)"/>
    public void PushMessage(TWrite message, int targetId)
    {
        lock (_connections)
        {
            // Can we speed this up with Linq or does that add overhead?
            foreach (NamedPipeConnection<TRead, TWrite> client in _connections)
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
    /// Sends a message to the specified clients asynchronously.
    /// </summary>
    /// <param name="targetIds">
    /// An array of client IDs to send the message to.
    /// </param>
    /// <inheritdoc cref="PushMessage(TWrite)"/>
    public void PushMessage(TWrite message, int[] targetIds)
    {
        PushMessage(message, targetIds.ToList());
    }

    /// <param name="targetIds">
    /// A list of client IDs to send the message to.
    /// </param>
    /// <inheritdoc cref="PushMessage(TWrite, int[])"/>
    public void PushMessage(TWrite message, List<int> targetIds)
    {
        lock (_connections)
        {
            // Can we speed this up with Linq or does that add overhead?
            foreach (NamedPipeConnection<TRead, TWrite> client in _connections)
            {
                if (targetIds.Contains(client.ID))
                {
                    client.PushMessage(message);
                }
            }
        }
    }

    /// <param name="targetName">
    /// The client name to send the message to.
    /// </param>
    /// <inheritdoc cref="PushMessage(TWrite, int)"/>
    public void PushMessage(TWrite message, string targetName)
    {
        lock (_connections)
        {
            // Can we speed this up with Linq or does that add overhead?
            foreach (NamedPipeConnection<TRead, TWrite> client in _connections)
            {
                if (client.Name == targetName)
                {
                    client.PushMessage(message);
                    break;
                }
            }
        }
    }

    /// <param name="targetNames">
    /// A list of client names to send the message to.
    /// </param>
    /// <inheritdoc cref="PushMessage(TWrite, List{int})"/>
    public void PushMessage(TWrite message, List<string> targetNames)
    {
        lock (_connections)
        {
            foreach (NamedPipeConnection<TRead, TWrite> client in _connections)
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
            foreach (NamedPipeConnection<TRead, TWrite> client in _connections.ToArray())
            {
                client.Close();
            }
        }

        // If background thread is still listening for a client to connect,
        // initiate a dummy connection that will allow the thread to exit.
        NamedPipeClient<TRead, TWrite> dummyClient = new(_pipeName);
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
        NamedPipeConnection<TRead, TWrite> connection = null;

        string connectionPipeName = GetNextConnectionPipeName();

        try
        {
            dataPipe = CreatePipe(connectionPipeName);

            // Send the client the name of the data pipe to use
            handshakePipe = CreateAndConnectPipe();

            PipeStreamWrapper<string, string> handshakeWrapper = new(handshakePipe);

            handshakeWrapper.WriteObject(connectionPipeName);
            handshakeWrapper.WaitForPipeDrain();
            handshakeWrapper.Close();


            // Wait for the client to connect to the data pipe
            dataPipe.WaitForConnection();

            // Add the client's connection to the list of connections
            connection = ConnectionFactory.CreateConnection<TRead, TWrite>(dataPipe);
            connection.ReceiveMessage += ClientOnReceiveMessage;
            connection.Disconnected += ClientOnDisconnected;
            connection.Error += ConnectionOnError;
            connection.Open();

            lock (_connections) { _connections.Add(connection); }

            PipeConnectionEventArgs<TRead, TWrite> e = new(connection);

            ClientOnConnected(this, e);
        }
        // Catch the IOException that is raised if the pipe is broken or disconnected.
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Named pipe is broken or disconnected: {ex}");

            Cleanup(handshakePipe);
            Cleanup(dataPipe);

            PipeConnectionEventArgs<TRead, TWrite> e = new(connection);

            ClientOnDisconnected(this, e);
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

    private void ClientOnConnected(object sender, PipeConnectionEventArgs<TRead, TWrite> e)
    {
        ClientConnected?.Invoke(sender, e);
    }

    private void ClientOnReceiveMessage(object sender, PipeMessageEventArgs<TRead, TWrite> e)
    {
        ClientMessage?.Invoke(sender, e);
    }

    private void ClientOnDisconnected(object sender, PipeConnectionEventArgs<TRead, TWrite> e)
    {
        if (e.Connection == null)
        {
            return;
        }

        lock (_connections)
        {
            _connections.Remove(e.Connection);
        }

        ClientDisconnected?.Invoke(sender, e);
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
    /// <param name="exception"></param>
    private void WorkerOnError(object sender, WorkerErrorEventArgs e)
    {
        PipeErrorEventArgs<TRead, TWrite> e2 = new(null, e.Exception);
        Error?.Invoke(sender, e2);
    }

    private string GetNextConnectionPipeName()
    {
        return $"{_pipeName}_{++_nextPipeId}";
    }

    private static void Cleanup(NamedPipeServerStream pipe)
    {
        if (pipe is null)
        {
            return;
        }

        using (NamedPipeServerStream x = pipe)
        {
            x.Close();
        }
    }

    #endregion
}
