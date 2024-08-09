using System.IO.Pipes;

namespace YAMDCC.IPC
{
    internal static class PipeServerFactory
    {
        public static NamedPipeServerStream CreateAndConnectPipe(string pipeName)
        {
            NamedPipeServerStream pipe = CreatePipe(pipeName);
            pipe.WaitForConnection();

            return pipe;
        }

        public static NamedPipeServerStream CreatePipe(string pipeName)
        {
            return new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
        }

        public static NamedPipeServerStream CreateAndConnectPipe(string pipeName, int bufferSize, PipeSecurity security)
        {
            NamedPipeServerStream pipe = CreatePipe(pipeName, bufferSize, security);
            pipe.WaitForConnection();

            return pipe;
        }

        public static NamedPipeServerStream CreatePipe(string pipeName, int bufferSize, PipeSecurity security)
        {
            return new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message,
                PipeOptions.Asynchronous, bufferSize, bufferSize, security);
        }
    }
}
