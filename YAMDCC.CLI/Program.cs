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
using System.Collections.Generic;
using YAMDCC.Common;

namespace YAMDCC.CLI;

internal static class Program
{
    /// <summary>
    /// The arguments passed to this program.
    /// Keys contain a verb (e.g. `help`), Values contain the
    /// arguments passed with the verb.
    /// </summary>
    private static readonly Dictionary<string, string> Args = [];

    private static void Main(string[] args)
    {
        // Parse the entire command line
        ParseArgs(args);

        if (Args.ContainsKey("license") || Args.ContainsKey("L"))
        {
            Console.WriteLine(Strings.GetString("Title", Utils.GetVerString()));
            Console.WriteLine(Strings.GetString("Copyright"));
            return;
        }
        else if (Args.ContainsKey("version") || Args.ContainsKey("V"))
        {
            Console.WriteLine(Utils.GetVerString());
            return;
        }
        else if (Args.ContainsKey("nologo"))
        {
            // remove nologo arg so it doesn't interfere with a check later on
            Args.Remove("nologo");
        }
        else
        {
            // print progressively squashed YAMDCC logo if CMD window is very small
            if (Console.BufferWidth >= 80)
            {
                Console.WriteLine(Strings.GetString("Logo", Utils.GetVerString()));
            }
            else if (Console.BufferWidth >= 60)
            {
                Console.WriteLine(Strings.GetString("LogoS", Utils.GetVerString()));
            }
            else
            {
                Console.WriteLine(Strings.GetString("Title", Utils.GetVerString()));
            }
            Console.WriteLine(Strings.GetString("CopyrightS"));
        }

        if (Args.Count == 0)
        {
            Console.WriteLine("ERROR: no arguments");
            PrintHelp();
        }
        else if (Args.ContainsKey("help") || Args.ContainsKey("H"))
        {
            PrintHelp();
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine(
            $"Usage: {AppDomain.CurrentDomain.FriendlyName} <options> [...]\n\n" +
            "Program information:\n" +
            "  -H, -help\tPrint this help screen.\n" +
            "  -V, -version\tPrint YAMDCC's current version.\n" +
            "  -L, -license\tPrint full copyright and license information.\n\n" +
            "...that's it for now. More stuff to be implemented Soon (TM)." +
            "");
    }

    private static void ParseArgs(string[] args)
    {
        string verb = string.Empty,
            arg = string.Empty;

        // TODO: more efficient way to do this?
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--")
            {
                // stop parsing arguments once we encounter a -- on its own
                // (matches the behaviour of many other CLI apps)
                return;
            }
            else if (args[i].StartsWith("-", StringComparison.Ordinal) ||
                args[i].StartsWith("/", StringComparison.Ordinal))
            {
                if (!string.IsNullOrEmpty(verb))
                {
                    Args.Add(verb, arg);
                    arg = string.Empty;
                }
                verb = args[i].Substring(1);
            }
            else
            {
                arg = args[i];
            }
        }

        // needed to add last argument if any
        if (!string.IsNullOrEmpty(verb))
        {
            Args.Add(verb, arg);
            arg = string.Empty;
        }
    }
}
