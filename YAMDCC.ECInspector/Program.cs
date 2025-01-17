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
using System.ServiceProcess;
using System.Threading;
using YAMDCC.Common;
using YAMDCC.IPC;

namespace YAMDCC.ECInspector;

internal sealed class Program
{
    private static readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
        new("YAMDCC-Server");

    private static readonly Mutex LogMutex = new(false);

    private static readonly ECValue[] ECValues = new ECValue[256];

    private static int Main(string[] args)
    {
        if (!Utils.IsAdmin())
        {
            Console.WriteLine(Strings.GetString("NoAdmin"));
            return 255;
        }

        // check that YAMDCC service is running
        using (ServiceController yamdccSvc = new("yamdccsvc"))
        {
            try
            {
                if (yamdccSvc.Status == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine(Strings.GetString("SvcStopped"));
                    return 1;
                }
            }
            catch
            {
                Console.WriteLine(Strings.GetString("SvcNotFound"));
                return 1;
            }
        }

        if (args.Length == 0)
        {
            Console.WriteLine(Strings.GetString("NoCmd"));
            Help();
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
                Help();
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
                Console.WriteLine(Strings.GetString("NoCmd"));
                Help();
                return 2;
            default:
                Console.WriteLine(Strings.GetString("BadCmd", args[0]));
                Help();
                return 2;
        }
    }

    private static void Help()
    {
        Console.WriteLine(Strings.GetString("Help",
            Environment.OSVersion, Utils.GetVerString(),
            Utils.GetRevision(), AppDomain.CurrentDomain.FriendlyName));
    }

    private static bool ConnectService()
    {
        IPCClient.ServerMessage += ServerMessage;
        IPCClient.Error += IPCError;

        IPCClient.Start();
        if (!IPCClient.WaitForConnection(5000))
        {
            Console.WriteLine(Strings.GetString("SvcConnErr"));
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
        Console.WriteLine("|".PadLeft(5, '-').PadRight(53, '-'));

        for (int i = 0; i < 16; i++)
        {
            Console.WriteLine($" {i:X}0 |");
        }

        Console.WriteLine("\nPress Ctrl+C to exit");
        Console.CursorVisible = false;
        Console.CancelKeyPress += CancelKey;

        int j = 0;
        while (true)
        {
            if (loops != -1 && j >= loops)
            {
                break;
            }

            byte i = byte.MaxValue;
            do
            {
                i++;
                IPCClient.PushMessage(new ServiceCommand(Command.ReadECByte, i));
            }
            while (i < byte.MaxValue);

            Thread.Sleep(interval);
            j++;
        }

        Console.CursorVisible = true;
        IPCClient.Stop();
    }

    private static void CancelKey(object sender, ConsoleCancelEventArgs e)
    {
        Console.CursorVisible = true;
        IPCClient.Stop();
    }

    private static void ServerMessage(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
    {
        if (LogMutex.WaitOne())
        {
            try
            {
                object[] args = e.Message.Value;
                if (e.Message.Response == Response.ReadResult &&
                    args.Length == 2 && args[0] is byte reg && args[1] is byte value)
                {
                    int lowBits = reg & 0x0F,
                        hiBits = (reg & 0xF0) >> 4;
                    Console.SetCursorPosition(6 + lowBits * 3, 4 + hiBits);

                    ConsoleColor original = Console.ForegroundColor;


                    if (ECValues[reg].Value == value)
                    {
                        ECValues[reg].Age++;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        ECValues[reg].Value = value;
                        ECValues[reg].Age = 0;
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    if (value == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write($"{value:X2}");
                    Console.ForegroundColor = original;
                    Console.SetCursorPosition(0, 20);
                }
            }
            finally
            {
                LogMutex.ReleaseMutex();
            }
        }
    }

    private static void IPCError(object sender, PipeErrorEventArgs<ServiceResponse, ServiceCommand> e)
    {
        throw e.Exception;
    }
}
