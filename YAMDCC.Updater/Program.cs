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
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Dialogs;
using YAMDCC.Updater.GitHubApi;

namespace YAMDCC.Updater;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

        if (args.Length > 0)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "--autoupdate":
                    return BGUpdate(true) ? 0 : 1;
                case "--checkupdate":
                    return BGUpdate(false) ? 0 : 1;
                case "--setautoupdate":
                    if (Utils.IsAdmin() && args.Length >= 2 &&
                        bool.TryParse(args[1], out bool enabled))
                    {
                        Updater.SetAutoUpdateEnabled(enabled);
                        return 0;
                    }
                    Utils.ShowError("Could not set auto-update state!");
                    return 1;
                case "--setprerelease":
                    if (Utils.IsAdmin() && args.Length >= 2 &&
                        bool.TryParse(args[1], out enabled))
                    {
                        CommonConfig.SetPreRelease(enabled);
                        return 0;
                    }
                    Utils.ShowError("Could not set pre-release setting!");
                    return 1;
                case "--install":
                    // args: --install <oldPath> <updatePath> <destPath> <confPath>
                    if (args.Length >= 5)
                    {
                        InstallUpdate(args[1], args[2], args[3], args[4]);
                    }
                    return 0;
            }
        }

        // no arguments, just launch normally
        Application.Run(new UpdateForm());
        return 0;
    }

    private static void InstallUpdate(
        string oldPath, string updatePath, string destPath, string confPath)
    {
        ProgressDialog<bool> dlg = new();

        dlg.DoWork = () =>
        {
            // kill all running instances of YAMDCC
            dlg.Caption = "Closing all YAMDCC instances...";
            string[] names =
            [
                "ConfigEditor",
                "ec-inspect",
                // yamdccsvc is stopped in next section
            ];

            foreach (Process p in Process.GetProcesses())
            {
                foreach (string name in names)
                {
                    if (p.ProcessName == name)
                    {
                        // try to close the main window.
                        // If that doesn't work, kill the process instead
                        if (!p.CloseMainWindow() || !p.WaitForExit(3000))
                        {
                            p.Kill();
                        }
                    }
                }
            }

            bool svcRunning = Utils.ServiceRunning("yamdccsvc");

            // stop the YAMDCC service if it's running
            if (svcRunning)
            {
                dlg.Caption = "Stopping YAMDCC service...";
                if (!Utils.StopService("yamdccsvc"))
                {
                    Utils.ShowError("Failed to stop YAMDCC service!");
                }
            }

            // delete the old YAMDCC installation
            dlg.Caption = "Installing YAMDCC update...";
            DirectoryInfo di = new(destPath);
            foreach (FileInfo fi in di.GetFiles())
            {
                fi.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                if (dir.FullName != confPath &&
                    dir.FullName != oldPath &&
                    dir.FullName != updatePath)
                {
                    dir.Delete(true);
                }
            }

            // move updated YAMDCC into place
            di = new(updatePath);
            foreach (FileInfo fi in di.GetFiles())
            {
                fi.MoveTo(Path.Combine(destPath, fi.Name));
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                if (dir.Name == "Configs")
                {
                    // copy new configs individually since we can't
                    // merge directories without extra work
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        fi.MoveTo(Path.Combine(confPath, fi.Name));
                    }
                }
                else
                {
                    dir.MoveTo(Path.Combine(destPath, dir.Name));
                }
            }

            // restart the YAMDCC service if it was running before the update
            if (svcRunning)
            {
                dlg.Caption = "Starting YAMDCC service...";
                Utils.StartService("yamdccsvc");
            }

            // cleanup :)
            // (note: does not delete "Old" folder that we should be running from)
            dlg.Caption = "Cleaning up...";
            Directory.Delete(updatePath, true);

            return true;
        };
        dlg.ShowDialog();

        if (Utils.ShowInfo(
            "YAMDCC updated successfully!\n" +
            "Would you like to run the config editor now?", "Success!",
            MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            Process.Start(Path.Combine(destPath, "ConfigEditor"));
        }
    }

    private static bool BGUpdate(bool autoUpdate)
    {
        try
        {
            bool preRelease = CommonConfig.GetPreRelease();
            Release release = Updater.GetLatestReleaseAsync(preRelease).GetAwaiter().GetResult();

            if (release is null && !preRelease)
            {
                // there's no non-prerelease version yet, try to get latest pre-release
                release = Updater.GetLatestReleaseAsync(true).GetAwaiter().GetResult();
                if (release is null)
                {
                    Utils.ShowError("Failed to get latest YAMDCC release info!");
                    return false;
                }
            }

            if (!Updater.IsDevVersion(release) && Updater.IsUpdateAvailable(release))
            {
                Application.Run(new UpdateForm(release, autoUpdate));
            }
            else if (!autoUpdate)
            {
                Utils.ShowInfo("YAMDCC is up to date.", "Up to date");
            }
            return true;
        }
        catch (HttpRequestException ex)
        {
            Utils.ShowError("Failed to check for YAMDCC update!\n" +
                $"Details:\n{GetExceptionMsgs(ex)}");
            return false;
        }
    }

    private static string GetExceptionMsgs(Exception ex)
    {
        string str = $"{ex.GetType()}: {ex.Message}";
        if (ex.InnerException is not null)
        {
            str += $" ---> {GetExceptionMsgs(ex.InnerException)}";
        }
        return str;
    }

    private static void ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        new CrashDialog(e.Exception).ShowDialog();
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        new CrashDialog((Exception)e.ExceptionObject).ShowDialog();
    }
}
