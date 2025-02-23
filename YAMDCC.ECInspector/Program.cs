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
using System.Threading;
using YAMDCC.Common;
using YAMDCC.ECAccess;

namespace YAMDCC.ECInspector;

internal static class Program
{
    private static readonly EC _EC = new();

    private static int Main(string[] args)
    {
        if (!Utils.IsAdmin())
        {
            Console.WriteLine("ERROR: admin privileges required");
            return 255;
        }

        if (args.Length > 0 && args[0].Length > 0)
        {
            switch (args[0][0])
            {
                case 'v':
                case 'V':
                    Console.WriteLine(Utils.GetVerString());
                    return 0;
                case 'h':
                case 'H':
                    break;
                case 'd':
                case 'D':
                    return DumpEC(1000, 1) ? 0 : 1;
                case 'm':
                case 'M':
                    return DumpEC(1000, -1) ? 0 : 1;
                default:
                    Console.WriteLine($"ERROR: unknown command: {args[0]}");
                    break;
            }
        }
        else
        {
            Console.WriteLine("ERROR: no command specified");
        }
        Help();
        return 1;
    }

    private static void Help()
    {
        Console.WriteLine("\nYAMDCC EC inspection utility\n\n" +
            $"OS version: {Environment.OSVersion}\n" +
            $"App version: {Utils.GetVerString()}\n" +
            $"Revision (git): {Utils.GetRevision()}\n\n" +
            $"Usage: {AppDomain.CurrentDomain.FriendlyName} <command> [<args>]\n\n" +
            "Commands:\n" +
            "  help              Print this help screen\n" +
            "  version           Print the program version\n" +
            "  dump              Dump all EC registers\n" +
            "  monitor           Dump EC and monitor for changes");
    }

    private static bool DumpEC(int interval, int loops)
    {
        if (!_EC.LoadDriver())
        {
            return false;
        }

        ECValue[] values = new ECValue[256];
        for (int i = 0; i <= byte.MaxValue; i++)
        {
            values[i] = new ECValue();
        }

        Console.Clear();
        Console.SetCursorPosition(0, 0);

        // write heading
        Console.Write("YAMDCC EC inspector\n\n 0x |");
        for (int i = 0; i < 16; i++)
        {
            Console.Write($" 0{i:X}");
        }
        Console.WriteLine("\n-----|".PadRight(53, '-'));

        for (int i = 0; i < 16; i++)
        {
            Console.WriteLine($" {i:X}0 |");
        }

        Console.WriteLine("Press Ctrl+C to exit");
        Console.CursorVisible = false;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKey);

        int j = 0;
        while (true)
        {
            if (loops != -1 && j >= loops)
            {
                break;
            }

            // TODO: optimise out jumping all over the place?
            // (leftover from when we used YAMDCC service for EC access)
            for (int i = 0; i < values.Length; i++)
            {
                if (_EC.ReadByte((byte)i, out byte value))
                {
                    int lowBits = i & 0x0F,
                        hiBits = (i & 0xF0) >> 4;

                    // keep the default console colour in case it was
                    // changed with e.g. the `color` command
                    ConsoleColor original = Console.ForegroundColor;

                    // write hex value
                    Console.SetCursorPosition(6 + lowBits * 3, 4 + hiBits);

                    if (values[i].Value == value)
                    {
                        values[i].Age++;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        values[i].Value = value;
                        values[i].Age = 0;
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    if (value == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write($"{value:X2}");

                    // write string representation
                    Console.SetCursorPosition(55 + lowBits, 4 + hiBits);
                    if (value < 32 || value > 126)
                    {
                        // unprintable non-extended ASCII char
                        Console.Write('.');
                    }
                    else
                    {
                        Console.Write((char)value);
                    }

                    // restore console colour
                    Console.ForegroundColor = original;
                }
            }

            Thread.Sleep(interval);
            j++;
        }

        Console.CursorVisible = true;
        _EC?.UnloadDriver();
        return true;
    }

    private static void CancelKey(object sender, ConsoleCancelEventArgs e)
    {
        Console.CursorVisible = true;
        _EC?.UnloadDriver();
    }

    private struct ECValue
    {
        /// <summary>
        /// The EC value itself.
        /// </summary>
        public int Value;

        /// <summary>
        /// How many EC polls it's been since <see cref="Value"/> was last updated.
        /// </summary>
        public int Age;
    }
}
