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
                    // args: --install <oldPath> <updatePath> <destPath>
                    if (args.Length >= 4)
                    {
                        InstallUpdate(args[1], args[2], args[3]);
                    }
                    return 0;
            }
        }

        // no arguments, just launch normally
        Application.Run(new UpdateForm());
        return 0;
    }

    private static void InstallUpdate(
        string oldPath, string updatePath, string destPath)
    {
        ProgressDialog<bool> dlg = new("Installing YAMDCC update...", () =>
        {
            bool svcRunning = Utils.ServiceRunning("yamdccsvc");

            // stop the YAMDCC service if it's running
            if (svcRunning && !Utils.StopService("yamdccsvc"))
            {
                Utils.ShowError("Failed to stop YAMDCC service!");
            }

            // delete the old YAMDCC installation
            DirectoryInfo di = new(destPath);
            foreach (FileInfo fi in di.GetFiles())
            {
                fi.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                if (dir.FullName != oldPath &&
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
                dir.MoveTo(Path.Combine(destPath, dir.Name));
            }

            // restart the YAMDCC service if it was running before the update
            if (svcRunning)
            {
                Utils.StartService("yamdccsvc");
            }

            // cleanup :)
            // (note: does not delete "Old" folder that we should be running from)
            Directory.Delete(updatePath);

            return true;
        });
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

            if (Utils.GetCurrentVersion() < Utils.GetVersion(release.TagName.Remove(0, 1)))
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
