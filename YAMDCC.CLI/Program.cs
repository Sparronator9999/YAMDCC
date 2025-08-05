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

        int fanIdx = -1,
            profIdx = -1,

            spdIdx = -1,
            fanSpd = -1,
            tUp = -1,
            tDown = -1,
            chargeLim = -1,
            perfMode = -2,  // -1 is used as "default" in fan profile settings
            keyLight = -1;

        string fanName = string.Empty,
            profName = string.Empty,
            confPath = Paths.CurrentConf,
            cfgAuthor = string.Empty;

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
        YAMDCC_Config cfg;
        try
        {
            cfg = YAMDCC_Config.Load(confPath);
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

        // Look up fan and profile indexes by name if fanIdx/profIdx are -1:
        if (fanIdx == -1)
        {
            for (int i = 0; i < cfg.FanConfs.Count; i++)
            {
                if (cfg.FanConfs[i].Name == fanName)
                {
                    fanIdx = i;
                    break;
                }
            }
        }

        if (profIdx == -1 && fanIdx != -1)
        {
            FanConf fanCfg =  cfg.FanConfs[fanIdx];
            for (int i = 0; i < fanCfg.FanCurveConfs.Count; i++)
            {
                if (fanCfg.FanCurveConfs[i].Name == profName)
                {
                    profIdx = i;
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
            Console.WriteLine("   Fan");
            Console.WriteLine("  Temp");
            Console.WriteLine(" Speed");
            Console.WriteLine("   RPM");
            Console.WriteLine();
            Console.ForegroundColor = fgColor;
            Console.WriteLine("Press Ctrl+C to exit");

            for (int i = 0; i < cfg.FanConfs.Count; i++)
            {
                Console.SetCursorPosition((i + 1) * 10, 2);
                Console.Write(cfg.FanConfs[i].Name);
            }

            while (true)
            {
                for (int i = 0; i < cfg.FanConfs.Count; i++)
                {
                    IPCClient.PushMessage(new ServiceCommand(Command.GetTemp, i));
                    IPCClient.PushMessage(new ServiceCommand(Command.GetFanSpeed, i));
                    IPCClient.PushMessage(new ServiceCommand(Command.GetFanRPM, i));
                }

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
                Console.WriteLine($"EC firmware version: {(cfg.FirmVerSupported ? cfg.FirmVer : "(unsupported)")}");
                Console.WriteLine($"EC firmware date: {(cfg.FirmVerSupported ? cfg.FirmDate : "(unsupported)")}");

                for (int i = 0; i < cfg.FanConfs.Count; i++)
                {
                    FanConf fanCfg = cfg.FanConfs[i];
                    FanCurveConf curveCfg = fanCfg.FanCurveConfs[fanCfg.CurveSel];

                    Console.WriteLine();
                    Console.WriteLine($"~~~~~ {i}: {fanCfg.Name} ~~~~~");
                    Console.WriteLine($"Fan speed range: {fanCfg.MinSpeed}-{fanCfg.MaxSpeed}%");
                    Console.WriteLine($"Current fan profile: {curveCfg.Name}");
                    Console.WriteLine("Available fan profiles:");
                    for (int j = 0; j < fanCfg.FanCurveConfs.Count; j++)
                    {
                        Console.WriteLine($"  {j}: {fanCfg.FanCurveConfs[j].Name}");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Current fan profile settings:");

                    StringBuilder
                        fanSpds = new("  Speed (%): "),
                        tUps =      new("    Up (°C): "),
                        tDowns =    new("  Down (°C): ");

                    for (int j = 0; j < curveCfg.TempThresholds.Count; j++)
                    {
                        fanSpds.Append($"{curveCfg.TempThresholds[j].FanSpeed,3} ");
                        if (j == 0)
                        {
                            tUps.Append("  L ");
                        }
                        else
                        {
                            tUps.Append($"{curveCfg.TempThresholds[j].UpThreshold,3} ");
                        }

                        if (j == curveCfg.TempThresholds.Count - 1)
                        {
                            tDowns.Append("  H ");
                        }
                        else
                        {
                            tDowns.Append($"{curveCfg.TempThresholds[j + 1].DownThreshold,3} ");
                        }
                    }
                    Console.WriteLine($"{fanSpds}");
                    Console.WriteLine($"{tUps}");
                    Console.WriteLine($"{tDowns}");
                }

                Console.WriteLine();
                if (cfg.ChargeLimitConf is not null ||
                    cfg.PerfModeConf is not null ||
                    cfg.KeySwapConf is not null)
                {
                    Console.WriteLine("~~~~~ Extras ~~~~~");
                    if (cfg.ChargeLimitConf is not null)
                    {
                        if (cfg.ChargeLimitConf.CurVal == 0)
                        {
                            Console.WriteLine("Charge threshold: disabled");
                        }
                        Console.WriteLine($"Charge threshold: {cfg.ChargeLimitConf.CurVal}%");
                    }
                    if (cfg.KeySwapConf is not null)
                    {
                        Console.WriteLine($"Win/Fn key swap: {(cfg.KeySwapConf.Enabled ? "enabled" : "disabled")}");
                    }
                    if (cfg.PerfModeConf is not null)
                    {
                        Console.WriteLine();
                        PerfModeConf pMode = cfg.PerfModeConf;
                        Console.WriteLine($"Default performance mode: {pMode.PerfModes[pMode.ModeSel].Name}");
                        Console.WriteLine("Available performance modes:");
                        for (int i = 0; i < pMode.PerfModes.Count; i++)
                        {
                            Console.WriteLine($"  {i}: {pMode.PerfModes[i].Name}");
                        }
                    }
                }
            }
        }

        // -new
        if (newFanProf)
        {
            for (int i = 0; i < cfg.FanConfs.Count; i++)
            {
                FanConf fanCfg = cfg.FanConfs[i];

                // Create a copy of the currently selected fan profile
                // and add it to the config's list:
                FanCurveConf oldCurveCfg = fanCfg.FanCurveConfs[fanCfg.CurveSel];
                fanCfg.FanCurveConfs.Add(oldCurveCfg.Copy());
                fanCfg.CurveSel = fanCfg.FanCurveConfs.Count - 1;

                // change name to indicate the new fan profile is a copy of the old one
                // TODO: allow profile name and description to be configured
                fanCfg.FanCurveConfs[fanCfg.CurveSel].Name = $"Copy of {oldCurveCfg.Name}";
                fanCfg.FanCurveConfs[fanCfg.CurveSel].Desc = $"(Copy of {oldCurveCfg.Name})\n{oldCurveCfg.Desc}";
            }
        }
        // -delete
        else if (delFanProf)
        {
            // Remove each equivalent fan profile from the config's list
            for (int i = 0; i < cfg.FanConfs.Count; i++)
            {
                FanConf fanCfg = cfg.FanConfs[i];
                fanCfg.FanCurveConfs.RemoveAt(fanCfg.CurveSel);
                if (fanCfg.CurveSel > 0)
                {
                    fanCfg.CurveSel -= 1;
                }
            }
        }

        // -speed
        if (spdIdx != -1)
        {
            TempThreshold t = cfg.FanConfs[fanIdx].FanCurveConfs[profIdx]
                .TempThresholds[spdIdx];

            t.FanSpeed = (byte)fanSpd;
            if (tUp != -1)
            {
                t.UpThreshold = (byte)tUp;
            }
            if (tDown != -1)
            {
                t.UpThreshold = (byte)tDown;
            }
        }

        // -chargelim
        if (chargeLim != -1)
        {
            int max = cfg.ChargeLimitConf.MaxVal - cfg.ChargeLimitConf.MinVal;
            if (chargeLim < 0 || chargeLim > max)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Strings.GetString("errChgLimVal", max));
                Console.ForegroundColor = fgColor;
                return;
            }
            cfg.ChargeLimitConf.CurVal = (byte)chargeLim;
        }

        // -perfmode
        if (perfMode != -2)
        {
            int max = cfg.PerfModeConf.PerfModes.Count;
            if (profIdx == -1)
            {
                // set global performance mode value by default
                if (perfMode < 0 || perfMode > max)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(Strings.GetString("errPMVal", 0, max));
                    Console.ForegroundColor = fgColor;
                    return;
                }
                cfg.PerfModeConf.ModeSel = (byte)perfMode;
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
                cfg.FanConfs[0].FanCurveConfs[profIdx].PerfModeSel = (byte)perfMode;
            }
        }

        // -keylight
        if (keyLight != -1 && ConnectSvc())
        {
            int max = cfg.KeyLightConf.MaxVal - cfg.KeyLightConf.MinVal;
            if (keyLight < 0 || keyLight > max)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Strings.GetString("errKLVal", max));
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
            if (confPath != Paths.CurrentConf)
            {
                cfg.Save(Paths.CurrentConf);
            }
            IPCClient.PushMessage(new ServiceCommand(Command.ApplyConf));
        }
        #endregion
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
