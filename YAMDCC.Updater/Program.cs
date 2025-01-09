using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Dialogs;
using Application = System.Windows.Forms.Application;

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
                        bool.TryParse(args[1], out bool enable))
                    {
                        Updater.SetAutoUpdateEnabled(enable);
                        return 0;
                    }
                    Utils.ShowError("Could not set auto-update state!");
                    return 1;
                case "--install":
                    // args: oldPath, updatePath, destPath
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
            Release[] releases = Updater.GetReleasesAsync(Utils.GetCurrentVerSuffix() != "release").GetAwaiter().GetResult();

            if (Utils.GetCurrentVersion() < Utils.GetVersion(releases[0].TagName.Remove(0, 1)))
            {
                Application.Run(new UpdateForm(releases, autoUpdate));
            }
            else if (!autoUpdate)
            {
                Utils.ShowInfo("YAMDCC is up to date.", "Up to date");
            }
            return true;
        }
        catch (Exception ex)
        {
            if (ex is HttpRequestException or ApiException)
            {
                Utils.ShowError("Failed to check for YAMDCC update!\n" +
                    $"Details:\n{GetExceptionMsgs(ex)}");
                return false;
            }
            else
            {
                throw;
            }
        }
    }

    private static string GetExceptionMsgs(Exception ex)
    {
        return ex.InnerException is null
            ? $"{ex.GetType()}: {ex.Message}"
            : $"{ex.GetType()}: {ex.Message} ---> {GetExceptionMsgs(ex.InnerException)}";
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
        else
        {
            return null;
        }
    }
}
