using System.IO.Pipes;

namespace YAMDCC.IPC;

internal static class ConnectionFactory
{
    private static int _lastId;

    internal static NamedPipeConnection<TRead, TWrite> CreateConnection<TRead, TWrite>(PipeStream pipeStream)
        where TRead : class
        where TWrite : class
    {
        return new NamedPipeConnection<TRead, TWrite>(++_lastId, $"Client {_lastId}", pipeStream);
    }
}
