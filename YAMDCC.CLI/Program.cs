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
using System.Text;
using System.Threading;
using YAMDCC.Common;
using YAMDCC.Common.Configs;
using YAMDCC.IPC;

namespace YAMDCC.CLI;

internal static class Program
{
    /// <summary>
    /// The arguments passed to this program.
    /// Keys contain a verb (e.g. `help`), Values contain the
    /// arguments passed with the verb.
    /// </summary>
    private static readonly Dictionary<string, string> Args = [];

    /// <summary>
    /// The client that connects to the YAMDCC Service
    /// </summary>
    private static readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
        new("YAMDCC-Server");

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

        int profSwitchIdx = -1,
            profEditIdx = -1,
            fanEditIdx = -1,


            spdIdx = -1,
            fanSpd = -1,
            tUp = -1,
            tDown = -1,
            chargeLim = -1,
            perfMode = -2,  // -1 is used as "default" in fan profile settings
            keyLight = -1;

        string fanName = string.Empty,
            profName = string.Empty,
            confPath = Paths.CurrentConfV2,
            cfgAuthor = string.Empty;

        #region Parse actual commands and arguments
        ConsoleColor fgColor = Console.ForegroundColor;
        foreach ((string verb, string arg) in Args)
        {
            // Check for missing or unknown arguments
            switch (verb)
            {
                case "E":
                case "edit":
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
                case "E":
                case "edit":
                    string[] profEditArgs = arg.Split(',');
                    if (profEditArgs.Length != 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {(profEditArgs.Length < 2 ? "Not enough" : "Too many")} arguments for `{verb}`");
                        Console.WriteLine($"(expected 2, got {profEditArgs.Length})");
                        Console.ForegroundColor = fgColor;
                        return;
                    }

                    switch (profEditArgs[0].ToLowerInvariant())
                    {
                        case "cpu":
                            fanEditIdx = 0;
                            break;
                        case "gpu":
                            fanEditIdx = 1;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"ERROR: unexpected value `{profEditArgs[0]}` when parsing arguments for `{verb}`");
                            Console.WriteLine("(expected either `cpu` or `gpu`)");
                            Console.ForegroundColor = fgColor;
                            return;
                    }

                    if (!int.TryParse(profEditArgs[1], out profEditIdx))
                    {
                        profName = profEditArgs[1];
                    }
                    break;
                case "P":
                case "profile":
                    if (!int.TryParse(arg, out profSwitchIdx))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Strings.GetString("errArgParse", verb));
                        Console.ForegroundColor = fgColor;
                        return;
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
                    if (spdArgs.Length < 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: Not enough arguments for `{verb}`");
                        Console.WriteLine($"(expected 2-4, got {spdArgs.Length})");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    else if (spdArgs.Length > 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: Too many arguments for `{verb}`");
                        Console.WriteLine($"(expected 2-4, got {spdArgs.Length})");
                        Console.ForegroundColor = fgColor;
                        return;
                    }

                    if (!int.TryParse(spdArgs[0], out spdIdx) ||
                       !int.TryParse(spdArgs[1], out fanSpd) ||
                       spdArgs.Length >= 3 && !int.TryParse(spdArgs[2], out tUp) ||
                       spdArgs.Length >= 4 && !int.TryParse(spdArgs[3], out tDown))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Strings.GetString("errArgParse", verb));
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "chargelim":
                    if (!int.TryParse(arg, out chargeLim))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Strings.GetString("errArgParse", verb));
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "perfmode":
                    if (!int.TryParse(arg, out perfMode))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Strings.GetString("errArgParse", verb));
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    break;
                case "keylight":
                    if (!int.TryParse(arg, out keyLight))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(Strings.GetString("errArgParse", verb));
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
                    if (string.IsNullOrEmpty(arg))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ERROR: `-author` requires a name argument");
                        Console.ForegroundColor = fgColor;
                        return;
                    }
                    cfgAuthor = arg;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: Unknown argument `{verb}`");
                    Console.ForegroundColor = fgColor;
                    continue;
            }
        }

        // stop here if we ran into command parsing errors
        // (done so we can show multiple parsing errors at once)
        if (error)
        {
            return;
        }
        #endregion

        #region The actual command logic
        // load the current YAMDCC config, or
        // the one specified by -config if it's set
        YamdccCfg cfg;
        try
        {
            cfg = YamdccCfg.Load(confPath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: Failed to load config at {confPath}!");
            if (!Args.ContainsKey("config") && !Args.ContainsKey("C"))
            {
                Console.WriteLine("Try loading a config using `-config <path>`.");
            }
            Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            Console.ForegroundColor = fgColor;
            return;
        }

        if (profEditIdx == -1 && fanEditIdx != -1)
        {
            FanConf fanCfg = fanEditIdx == 1 ? cfg.GpuFan : cfg.CpuFan;
            for (int i = 0; i < fanCfg.FanProfs.Count; i++)
            {
                if (fanCfg.FanProfs[i].Name == profName)
                {
                    profEditIdx = i;
                    break;
                }
            }
        }

        // -monitor
        if (ecMonitor && ConnectSvc())
        {
            Console.Clear();
            Console.WriteLine("YAMDCC EC monitor");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("   Fan    CPU         GPU");
            Console.WriteLine("  Temp");
            Console.WriteLine(" Speed");
            Console.WriteLine("   RPM");
            Console.WriteLine();
            Console.ForegroundColor = fgColor;
            Console.WriteLine("Press Ctrl+C to exit");

            while (true)
            {
                IPCClient.PushMessage(new ServiceCommand(Command.GetTemp, false));
                IPCClient.PushMessage(new ServiceCommand(Command.GetFanSpeed, false));
                IPCClient.PushMessage(new ServiceCommand(Command.GetFanRPM, false));
                IPCClient.PushMessage(new ServiceCommand(Command.GetTemp, true));
                IPCClient.PushMessage(new ServiceCommand(Command.GetFanSpeed, true));
                IPCClient.PushMessage(new ServiceCommand(Command.GetFanRPM, true));
                Thread.Sleep(1000);
            }
        }

        // -info
        if (showInfo)
        {
            if (cfg is null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Strings.GetString("errNoCfg"));
                Console.ForegroundColor = fgColor;
            }
            else
            {
                Console.WriteLine("~~~~~ Config info ~~~~~");
                Console.WriteLine($"Author: {cfg.Author}");
                Console.WriteLine($"Laptop manufacturer: {cfg.Manufacturer}");
                Console.WriteLine($"Laptop model: {cfg.Model}");
                Console.WriteLine($"EC firmware version: {cfg.FirmVer}");
                Console.WriteLine($"EC firmware date: {cfg.FirmDate}");

                WriteConfInfo(cfg.CpuFan, false);
                WriteConfInfo(cfg.GpuFan, true);

                Console.WriteLine();
                Console.WriteLine("~~~~~ Extras ~~~~~");
                if (cfg.ChargeLim == 0)
                {
                    Console.WriteLine("Charge threshold: disabled");
                }
                Console.WriteLine($"Charge threshold: {cfg.ChargeLim}%");

                Console.WriteLine($"Win/Fn key swap: {(cfg.KeySwapEnabled ? "enabled" : "disabled")}");

                Console.WriteLine();
                Console.WriteLine($"Default performance mode: {cfg.PerfMode}");
            }
        }

        // -new
        if (newFanProf)
        {
            CloneFanProf(cfg.CpuFan);
            CloneFanProf(cfg.GpuFan);
        }

        // -delete
        else if (delFanProf)
        {
            // Remove each equivalent fan profile from the config's list
            FanConf fanCfg = cfg.CpuFan;
            fanCfg.FanProfs.RemoveAt(cfg.CpuFan.ProfSel);
            if (fanCfg.ProfSel > 0)
            {
                fanCfg.ProfSel -= 1;
            }
            fanCfg = cfg.GpuFan;
            fanCfg.FanProfs.RemoveAt(cfg.CpuFan.ProfSel);
            if (fanCfg.ProfSel > 0)
            {
                fanCfg.ProfSel -= 1;
            }
        }

        // -profile
        if (profSwitchIdx != -1)
        {
            cfg.CpuFan.ProfSel = profSwitchIdx;
            cfg.GpuFan.ProfSel = profSwitchIdx;
        }

        // -speed
        if (spdIdx != -1)
        {
            FanConf fanCfg = fanEditIdx == 1 ? cfg.GpuFan : cfg.CpuFan;
            Threshold t = fanCfg.FanProfs[profEditIdx].Thresholds[spdIdx];

            t.Speed = (byte)fanSpd;
            if (tUp != -1)
            {
                t.Tup = (byte)tUp;
            }
            if (tDown != -1)
            {
                t.Tup = (byte)tDown;
            }
        }

        // -chargelim
        if (chargeLim != -1)
        {
            if (chargeLim < 0 || chargeLim > 100)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Strings.GetString("errChgLimVal"));
                Console.ForegroundColor = fgColor;
                return;
            }
            cfg.ChargeLim = (byte)chargeLim;
        }

        // -perfmode
        if (perfMode != -2)
        {
            int max = Enum.GetValues(typeof(PerfMode)).Length;
            if (profEditIdx == -1)
            {
                // set global performance mode value by default
                if (perfMode < 0 || perfMode > max)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Strings.GetString("errPMVal", 0, max));
                    Console.ForegroundColor = fgColor;
                    return;
                }
                cfg.PerfMode = (PerfMode)perfMode;
            }
            else
            {
                // otherwise set per-profile performance mode value
                if (perfMode < -1 || perfMode > max)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Strings.GetString("errPMVal", "-1 (default)", max));
                    Console.ForegroundColor = fgColor;
                    return;
                }
                // per-profile performance mode is always
                // applied from the first fan's config
                cfg.CpuFan.FanProfs[profEditIdx].PerfMode = (PerfMode)perfMode;
            }
        }

        // -keylight
        if (keyLight != -1 && ConnectSvc())
        {
            if (keyLight < 0 || keyLight > 4)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Strings.GetString("errKLVal", 4));
                Console.ForegroundColor = fgColor;
                return;
            }
            IPCClient.PushMessage(new ServiceCommand(Command.SetKeyLightBright, keyLight));
        }

        // -author
        if (!string.IsNullOrEmpty(cfgAuthor))
        {
            cfg.Author = cfgAuthor;
        }

        // save any modifications to the YAMDCC config
        try
        {
            cfg.Save(confPath);
        }
        catch (UnauthorizedAccessException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Strings.GetString("wrnCfgAdmin"));
            Console.ForegroundColor = fgColor;
        }

        // -apply
        if (applyConf && ConnectSvc())
        {
            // save config to be applied to the correct location if
            // editing a config other than CurrentConfig.xml
            if (confPath != Paths.CurrentConfV2)
            {
                cfg.Save(Paths.CurrentConfV2);
            }
            IPCClient.PushMessage(new ServiceCommand(Command.ApplyConf));
        }
        #endregion
    }

    private static void WriteConfInfo(FanConf cfg, bool gpu)
    {
        FanProf prof = cfg.FanProfs[cfg.ProfSel];

        Console.WriteLine();
        Console.WriteLine($"~~~~~ {(gpu ? "GPU" : "CPU")} fan ~~~~~");
        Console.WriteLine($"Current fan profile: {prof.Name}");
        Console.WriteLine("Available fan profiles:");
        for (int j = 0; j < cfg.FanProfs.Count; j++)
        {
            Console.WriteLine($"  {j}: {cfg.FanProfs[j].Name}");
        }

        Console.WriteLine();
        Console.WriteLine("Current fan profile settings:");

        StringBuilder
            fanSpds = new("  Speed (%): "),
            tUps =      new("    Up (°C): "),
            tDowns =    new("  Down (°C): ");

        for (int j = 0; j < prof.Thresholds.Count; j++)
        {
            fanSpds.Append($"{prof.Thresholds[j].Speed,3} ");
            if (j == 0)
            {
                tUps.Append("  L ");
            }
            else
            {
                tUps.Append($"{prof.Thresholds[j].Tup,3} ");
            }

            if (j == prof.Thresholds.Count - 1)
            {
                tDowns.Append("  H ");
            }
            else
            {
                tDowns.Append($"{prof.Thresholds[j + 1].Tdown,3} ");
            }
        }
        Console.WriteLine($"{fanSpds}");
        Console.WriteLine($"{tUps}");
        Console.WriteLine($"{tDowns}");
    }

    private static void CloneFanProf(FanConf cfg)
    {
        // Create a copy of the currently selected fan profile
        // and add it to the config's list:
        FanProf oldCurveCfg = cfg.FanProfs[cfg.ProfSel];
        cfg.FanProfs.Add(oldCurveCfg.Copy());
        cfg.ProfSel = cfg.FanProfs.Count - 1;

        // change name to indicate the new fan profile is a copy of the old one
        // TODO: allow profile name and description to be configured
        cfg.FanProfs[cfg.ProfSel].Name = $"Copy of {oldCurveCfg.Name}";
        cfg.FanProfs[cfg.ProfSel].Desc = $"(Copy of {oldCurveCfg.Name})\n{oldCurveCfg.Desc}";
    }

    private static void PrintHelp()
    {
        Console.WriteLine(Strings.GetString("Help", AppDomain.CurrentDomain.FriendlyName));
    }

    /// <summary>
    /// Connects to the YAMDCC service, if not already connected.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if connected successfully, otherwise <see langword="false"/>.
    /// </returns>
    private static bool ConnectSvc()
    {
        ConsoleColor fgColor = Console.ForegroundColor;
        if (!Utils.IsAdmin())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Strings.GetString("errSvcAdmin"));
            Console.ForegroundColor = fgColor;
            return false;
        }

        // return true if we already connected to the YAMDCC service
        if (IPCClient.Connection is not null && IPCClient.Connection.IsConnected)
        {
            return true;
        }

        IPCClient.ServerMessage += new EventHandler<PipeMessageEventArgs<ServiceResponse, ServiceCommand>>(IPCMessage);
        IPCClient.Start();
        Console.WriteLine("Connecting to YAMDCC service...");
        if (IPCClient.WaitForConnection(5000))
        {
            Console.WriteLine("Connected successfully!");
            return true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Strings.GetString("errSvcConn"));
            Console.ForegroundColor = fgColor;
            return false;
        }
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

    private static void IPCMessage(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
    {
        object[] args = e.Message.Value;
        switch (e.Message.Response)
        {
            case Response.Temp:
            {
                if (args.Length == 2 && args[0] is int fan && args[1] is int temp)
                {
                    Console.SetCursorPosition((fan + 1) * 10, 3);
                    Console.Write($"{temp,4} °C");
                }
                break;
            }
            case Response.FanSpeed:
            {
                if (args.Length == 2 && args[0] is int fan && args[1] is int speed)
                {
                    Console.SetCursorPosition((fan + 1) * 10, 4);
                    Console.Write($"{speed,4} %");
                }
                break;
            }
            case Response.FanRPM:
            {
                if (args.Length == 2 && args[0] is int fan && args[1] is int rpm)
                {
                    if (rpm < 0)
                    {
                        rpm = 0;
                    }
                    Console.SetCursorPosition((fan + 1) * 10, 5);
                    Console.Write($"{rpm,4} RPM");
                }
                break;
            }
        }
        Console.SetCursorPosition(0, 6);
    }
}
