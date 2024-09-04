using YAMDCC.IPC.IO;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;

namespace YAMDCC.IPC
{
    internal static class PipeClientFactory
    {
        internal static PipeStreamWrapper<TRead, TWrite> Connect<TRead, TWrite>(string pipeName)
            where TRead : class
            where TWrite : class
        {
            return new PipeStreamWrapper<TRead, TWrite>(CreateAndConnectPipe(pipeName));
        }

        internal static NamedPipeClientStream CreateAndConnectPipe(string pipeName, int timeout = 10)
        {
            string normalizedPath = Path.GetFullPath($"\\\\.\\pipe\\{pipeName}");
            while (!NamedPipeExists(normalizedPath))
            {
                Thread.Sleep(timeout);
            }
            NamedPipeClientStream pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
            pipe.Connect(1000);
            return pipe;
        }

        private static bool NamedPipeExists(string pipeName)
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

        [DllImport("kernel32.dll",
            CharSet = CharSet.Unicode,
            EntryPoint = "WaitNamedPipeW",
            SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WaitNamedPipe(string name, int timeout);
    }
}
