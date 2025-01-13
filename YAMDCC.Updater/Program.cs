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
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
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

        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

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
        ProgressDialog dlg = new("Installing YAMDCC update...", (e) =>
        {
            // uninstall the old YAMDCC service
            // TODO: detect if YAMDCC service is already uninstalled
            if (Utils.StopService("yamdccsvc"))
            {
                if (!Utils.UninstallService($"{destPath}\\yamdccsvc"))
                {
                    Utils.ShowError("Failed to uninstall YAMDCC service!");
                }
            }
            else
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

            // install the new YAMDCC service
            if (Utils.InstallService($"{destPath}\\yamdccsvc"))
            {
                Utils.StartService("yamdccsvc");
            }

            // cleanup :)
            // (note: does not delete "Old" folder that we should be running from)
            Directory.Delete(updatePath);
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
            Release release = Updater.GetLatestReleaseAsync(Utils.GetCurrentVerSuffix() != "release").GetAwaiter().GetResult();

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

    private static Assembly AssemblyResolve(object sender, ResolveEventArgs e)
    {
        // fix conflict between MessagePack's System.Runtime.CompilerServices.Unsafe
        // assembly (v6.0.0.0) and the version that Markdig needs (v4.0.4.1) that
        // can't be resolved with a binding redirect:
        if (e.Name == Resources.GetString("SRCSU_Name"))
        {
            // unpack gzipped System.Runtime.CompilerServices.Unsafe dll
            GZipStream input = new(new MemoryStream(Resources.GetObject("SRCSU_Dll")), CompressionMode.Decompress);
            MemoryStream output = new();
            input.CopyTo(output);

            return Assembly.Load(output.ToArray());
        }
        return null;
    }
}
