// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2025.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using YAMDCC.Common;
using YAMDCC.IPC;

namespace YAMDCC.ECInspector
{
    internal sealed class Program
    {
        private static readonly string ExeName =
            Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

        private static readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
            new("YAMDCC-Server");

        private static readonly Mutex LogMutex = new(false);

        private static readonly ECValue[] ECValues = new ECValue[256];

        private static int Main(string[] args)
        {
            if (!Utils.IsAdmin())
            {
                Console.WriteLine("ERROR: please re-run this program as admin.");
                return 255;
            }

            // check that YAMDCC service is running
            ServiceController yamdccSvc = new("yamdccsvc");
            try
            {
                if (yamdccSvc.Status == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine(
                        "ERROR: the YAMDCC service is not running.\n" +
                        "Please run the YAMDCC config editor to start the service.");
                    return 1;
                }
            }
            catch
            {
                Console.WriteLine(
                    "ERROR: the YAMDCC service is not installed.\n" +
                    "Please run the YAMDCC config editor to install the service.");
                return 1;
            }
            finally
            {
                yamdccSvc?.Close();
            }

            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: no command specified\n");
                PrintHelp();
                return 2;
            }
            switch (args[0])
            {
                case "--version":
                case "-v":
                    Console.WriteLine(Utils.GetVerString());
                    return 0;
                case "--help":
                case "-h":
                    PrintHelp();
                    return 0;
                case "--dump":
                case "-d":
                    if (ConnectService())
                    {
                        DumpEC(1000, 1);
                        return 0;
                    }
                    return 3;
                case "--monitor":
                case "-m":
                    if (ConnectService())
                    {
                        DumpEC(1000, -1);
                        return 0;
                    }
                    return 3;
                case "":
                    Console.WriteLine("ERROR: no command specified\n");
                    PrintHelp();
                    return 2;
                default:
                    Console.WriteLine($"ERROR: unknown command: {args[0]}\n");
                    PrintHelp();
                    return 2;
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(
                "YAMDCC EC inspection utility\n\n" +
                $"OS version: {Environment.OSVersion}\n" +
                $"App version: {Utils.GetVerString()}\n" +
                $"Revision (git): {Utils.GetRevision()}\n\n" +
                $"Usage: {ExeName} <command> [<args>]\n\n" +
                "Commands:\n\n" +
                "  --help, -h                      Print this help screen\n" +
                "  --version, -v                   Print the program version\n" +
                "  --dump, -d                      Dump all EC registers\n" +
                "  --monitor, -m                   Dump EC and monitor for changes\n" +
                "    --interval, -i <seconds>        EC polling interval");
        }

        private static bool ConnectService()
        {
            IPCClient.ServerMessage += IPCClient_ServerMessage;
            IPCClient.Error += IPCClient_Error;

            IPCClient.Start();
            if (!IPCClient.WaitForConnection(5000))
            {
                Console.WriteLine("ERROR: failed to connect to YAMDCC service!");
                return false;
            }
            return true;
        }

        private static void DumpEC(int interval, int loops)
        {
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                ECValues[i] = new ECValue();
            }
            Console.Clear();

            Console.SetCursorPosition(0, 0);

            // write heading
            Console.Write("YAMDCC EC inspector\n\n 0x |");
            for (int i = 0; i < 16; i++)
            {
                Console.Write($" 0{i:X}");
            }
            Console.WriteLine();
            Console.WriteLine("----|------------------------------------------------");

            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine($" {i:X}0 |");
            }

            Console.WriteLine("\nPress Ctrl+C to exit");
            Console.CursorVisible = false;
            Console.CancelKeyPress += Console_CancelKeyPress;

            int j = 0;
            while (true)
            {
                if (loops != -1 && j >= loops)
                {
                    break;
                }
                for (int i = 0; i <= byte.MaxValue; i++)
                {
                    IPCClient.PushMessage(new ServiceCommand(Command.ReadECByte, $"{i}"));
                }
                Thread.Sleep(interval);
                j++;
            }

            Console.CursorVisible = true;
            IPCClient.Stop();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.CursorVisible = true;
            IPCClient.Stop();
        }

        private static void IPCClient_ServerMessage(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
        {
            if (LogMutex.WaitOne())
            {
                try
                {
                    switch (e.Message.Response)
                    {
                        case Response.ReadResult:
                            if (ParseArgs(e.Message.Value, out int[] args) && args.Length == 2)
                            {
                                int lowBits = args[0] & 0x0F,
                                    hiBits = (args[0] & 0xF0) >> 4;
                                Console.SetCursorPosition(6 + lowBits * 3, 4 + hiBits);

                                ConsoleColor original = Console.ForegroundColor;


                                if (ECValues[args[0]].Value == args[1])
                                {
                                    ECValues[args[0]].Age++;
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                }
                                else
                                {
                                    ECValues[args[0]].Value = args[1];
                                    ECValues[args[0]].Age = 0;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                }

                                if (args[1] == 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                }
                                Console.Write($"{args[1]:X2}");
                                Console.ForegroundColor = original;
                                Console.SetCursorPosition(0, 20);
                            }
                            break;
                    }
                }
                finally
                {
                    LogMutex.ReleaseMutex();
                }
            }
        }

        private static void IPCClient_Error(object sender, PipeErrorEventArgs<ServiceResponse, ServiceCommand> e)
        {
            throw e.Exception;
        }

        private static bool ParseArgs(string argsIn, out int[] argsOut)
        {
            if (string.IsNullOrEmpty(argsIn))
            {
                argsOut = [];
            }
            else
            {
                string[] args_str = argsIn.Split(' ');
                argsOut = new int[args_str.Length];

                for (int i = 0; i < args_str.Length; i++)
                {
                    if (!int.TryParse(args_str[i], out argsOut[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
