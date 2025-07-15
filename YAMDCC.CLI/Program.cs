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
            // don't print logo if CMD window is too small
            if (Console.BufferWidth >= 80)
            {
                Console.WriteLine(Strings.GetString("Logo", Utils.GetVerString()));
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
            return;
        }
        else if (Args.ContainsKey("help") || Args.ContainsKey("H"))
        {
            PrintHelp();
            return;
        }

        bool error = false,
            showInfo = false,
            ecMonitor = false,
            newFanProf = false,
            delFanProf = false,
            applyConf = false;

        int fanIdx = -1,
            profIdx = -1,

            fanSpd = -1,
            tUp = -1,
            tDown = -1,
            chargeLim = -1,
            perfMode = -2,  // -1 is used as "default" in fan profile settings
            keyLight = -1;

        string fanName = string.Empty,
            profName = string.Empty,
            confPath = Paths.CurrentConf;

        #region Parse actual commands and arguments
        ConsoleColor fgColor = Console.ForegroundColor;
        foreach ((string verb, string arg) in Args)
        {
            // Check for missing or unknown arguments
            switch (verb)
            {
                case "F":
                case "fan":
                case "P":
                case "profile":
                case "S":
                case "speed":
                case "chargelim":
                case "perfmode":
                case "keylight":
                case "C":
                case "config":
                case "author":
                    if (string.IsNullOrEmpty(arg))
                    {
                        error = true;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: Missing argument for `{verb}`");
                        Console.ForegroundColor = fgColor;
                        continue;
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown argument `{verb}`");
                    Console.ForegroundColor = fgColor;
                    continue;
            }

            // actually parse arguments
            switch (verb)
            {
                case "I":
                case "info":
                    showInfo = true;
                    break;
                case "M":
                case "monitor":
                    ecMonitor = true;
                    break;
                case "F":
                case "fan":
                    // Try to access fan by index first, otherwise
                    // try accessing by name (done after parsing all arguments)
                    if (!int.TryParse(arg, out fanIdx))
                    {
                        fanName = arg;
                    }
                    break;
                case "P":
                case "profile":
                    // Same as "fan" but for fan profile.
                    if (!int.TryParse(arg, out profIdx))
                    {
                        profName = arg;
                    }
                    break;
                case "N":
                case "new":
                    delFanProf = false;
                    newFanProf = true;
                    break;
                case "D":
                case "delete":
                    newFanProf = false;
                    delFanProf = true;
                    break;
                case "S":
                case "speed":
                    string[] spdArgs = arg.Split(',');
                    if (spdArgs.Length < 2 || spdArgs.Length > 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: Not enough arguments for `{verb}`");
                        Console.WriteLine($"(expected 2-4, got {spdArgs.Length})");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    if (!int.TryParse(spdArgs[0], out fanIdx) ||
                       !int.TryParse(spdArgs[1], out fanSpd) ||
                       (spdArgs.Length >= 3 && !int.TryParse(spdArgs[2], out tUp)) ||
                       (spdArgs.Length >= 4 && !int.TryParse(spdArgs[3], out tDown)))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: failed to parse arguments for `{verb}`");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "chargelim":
                    if (!int.TryParse(arg, out chargeLim))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: failed to parse argument for `{verb}`");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "perfmode":
                    if (!int.TryParse(arg, out perfMode))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: failed to parse argument for `{verb}`");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "keylight":
                    if (!int.TryParse(arg, out keyLight))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: failed to parse argument for `{verb}`");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "C":
                case "config":
                    confPath = arg;
                    break;
                case "A":
                case "apply":
                    applyConf = true;
                    break;
                case "author":
                    break;
            }
        }

        // stop here if we ran into command parsing errors
        // (done so we can show multiple parsing errors at once)
        if (error)
        {
            return;
        }
        #endregion


    }

    private static void PrintHelp()
    {
        Console.WriteLine(Strings.GetString("Help", AppDomain.CurrentDomain.FriendlyName));
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
        }
    }

    // https://stackoverflow.com/a/42549535
    public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
    {
        key = tuple.Key;
        value = tuple.Value;
    }
}
