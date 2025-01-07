using System;

namespace YAMDCC.IPC;

/// <summary>
/// Provides data for the
/// <see cref="NamedPipeServer{TRead, TWrite}.ClientConnected"/>,
/// <see cref="NamedPipeServer{TRead, TWrite}.ClientDisconnected"/>, and
/// <see cref="NamedPipeClient{TRead, TWrite}.Disconnected"/> events.
/// </summary>
/// <typeparam name="TRead">
/// The reference type used when reading from the named pipe.
/// </typeparam>
/// <typeparam name="TWrite">
/// The reference type used when writing to the named pipe.
/// </typeparam>
public class PipeConnectionEventArgs<TRead, TWrite> : EventArgs
    where TRead : class
    where TWrite : class
{
    /// <summary>
    /// The connection that caused the (dis)connection event.
    /// </summary>
    public NamedPipeConnection<TRead, TWrite> Connection { get; }

    /// <summary>
    /// Initialises a new instance of the
    /// <see cref="PipeConnectionEventArgs{TRead, TWrite}"/> class.
    /// </summary>
    /// <param name="connection">
    /// The connection that should be associated with the event.
    /// </param>
    internal PipeConnectionEventArgs(NamedPipeConnection<TRead, TWrite> connection)
    {
        Connection = connection;
    }
}
