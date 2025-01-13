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

using MarkedNet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Updater.GitHubApi;

namespace YAMDCC.Updater;

internal sealed partial class UpdateForm : Form
{
    private static readonly Marked Markdown = new()
    {
        Options = new Options()
        {
            Gfm = true,
        },
    };

    private readonly bool AutoUpdate;

    private Release Release;

    private static readonly string ExeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
    private static readonly string TargetPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    private static readonly string DownloadPath = Path.GetFullPath("YAMDCC-Update.zip");
    private static readonly string UpdatePath = Path.GetFullPath("Update");
    private static readonly string OldPath = Path.GetFullPath("Old");

    public UpdateForm(Release release = null, bool autoUpdate = false)
    {
        InitializeComponent();
        Icon = Utils.GetEntryAssemblyIcon();
        AutoUpdate = autoUpdate;

        SetTitleText(string.Empty);
        tsiPreRelease.Checked = Utils.GetCurrentVerSuffix() != "release";
        tsiAutoUpdate.Checked = Updater.GetAutoUpdateEnabled();

        if (release is null)
        {
            wbChangelog.DocumentText = GetHtml(
                Strings.GetString("UpdatePrompt") +
                (tsiPreRelease.Checked ? Strings.GetString("PreReleaseOn") : string.Empty));
        }
        else
        {
            Release = release;
            UpdateAvailable();
        }
    }

    private void wbChangelog_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        string url = e.Url.ToString();

        if (url.StartsWith("yamdcc", StringComparison.OrdinalIgnoreCase))
        {
            switch (url.Remove(0, url.IndexOf(':') + 1))
            {
                case "reinstall":
                    UpdateAvailable();
                    e.Cancel = true;
                    break;
            }
        }

        // open external links in user's web browser
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            Process.Start(url);
            e.Cancel = true;
        }
    }

    private async void btnUpdate_Click(object sender, EventArgs e)
    {
        if ("update".Equals(btnUpdate.Tag) && Release is not null)
        {
            // disable buttons while updating
            btnUpdate.Enabled = false;
            btnOptions.Enabled = false;
            btnDisable.Enabled = false;
            btnLater.Enabled = false;

            SetProgress(-1, "Downloading update...");
            try
            {
                // delete old update ZIP if it's still there
                // (from cancelled/failed previous update)
                File.Delete(DownloadPath);
            }
            catch (FileNotFoundException) { }

            try
            {
                await Updater.DownloadUpdateAsync(
                    Release, DownloadPath, DownloadProgress);
            }
            catch (HttpRequestException ex)
            {
                UpdateError(ex, "failed to download update", "errDownload");
            }

            try
            {
                SetProgress(-1, "Extracting update...");
                await ExtractUpdate(DownloadPath, UpdatePath);
            }
            catch (Exception ex)
            {
                UpdateError(ex, "failed to extract update", "errExtract");
            }

            // delete old YAMDCC install from previous update if it exists
            SetProgress(-1, "Preparing to install update...");
            try
            {
                Directory.Delete(OldPath, true);
            }
            catch (DirectoryNotFoundException) { }

            // make a copy of the old YAMDCC installation
            CopyDir(new DirectoryInfo(TargetPath), new DirectoryInfo(OldPath));

            // actually install the update
            InstallUpdate();
            return;
        }
        else if ("install".Equals(btnUpdate.Tag))
        {
            InstallUpdate();
        }
        else
        {
            CheckUpdate();
        }
    }

    private void btnLater_Click(object sender, EventArgs e)
    {
        // just close the updater and let Windows task
        // scheduler do the reminding by re-running the updater
        Close();
    }

    private void btnDisable_Click(object sender, EventArgs e)
    {
        // disable auto-updates
        SetAutoUpdate(false);
        Close();
    }

    private void btnOptions_Click(object sender, EventArgs e)
    {
        OptMenu.Show(MousePosition);
    }

    private void tsiAutoUpdate_Click(object sender, EventArgs e)
    {
        if (SetAutoUpdate(!tsiAutoUpdate.Checked))
        {
            tsiAutoUpdate.Checked = !tsiAutoUpdate.Checked;
        }
    }

    private void tsiPreRelease_Click(object sender, EventArgs e)
    {
        if (btnUpdate.Tag is null)
        {
            wbChangelog.DocumentText = GetHtml(
                Strings.GetString("UpdatePrompt") +
                (tsiPreRelease.Checked ? Strings.GetString("PreReleaseOn") : string.Empty));
        }
    }

    private async void CheckUpdate()
    {
        btnUpdate.Enabled = false;
        btnOptions.Enabled = false;
        SetProgress(-1, "Checking for updates...");
        SetTitleText("Checking for updates");
        wbChangelog.DocumentText = GetHtml("Checking for updates...");

        try
        {
            Release = await Updater.GetLatestReleaseAsync(tsiPreRelease.Checked);
        }
        catch (HttpRequestException ex)
        {
            UpdateError(ex, "failed to check for updates", "errCheckUpdate");
            btnOptions.Enabled = true;
            return;
        }

        if (Release is null)
        {
            SetProgress(0, Strings.GetString("errNoReleaseS"));
            wbChangelog.DocumentText = GetHtml(Strings.GetString(
                "errNoRelease"));
        }
        else
        {
            Version current = Utils.GetCurrentVersion(),
                latest = Utils.GetVersion(Release.TagName.Remove(0, 1));

            if (current == latest)
            {
                SetProgress(100, "YAMDCC is up to date.");
                SetTitleText("Up to date");
                wbChangelog.DocumentText = GetHtml("YAMDCC is up to date.\n\n" +
                    "[Force-reinstall latest release?](yamdcc:reinstall)");
            }
            else if (current > latest)
            {
                SetProgress(100, "Current YAMDCC version > Latest YAMDCC version???");
                SetTitleText("Dev version detected");
                wbChangelog.DocumentText = GetHtml(
                    "You appear to be running a unreleased/development version of YAMDCC.\n\n" +
                    "[Update/downgrade to latest public release anyway?](yamdcc:reinstall)");
            }
            else
            {
                UpdateAvailable();
            }
        }
        btnUpdate.Enabled = true;
        btnOptions.Enabled = true;
    }

    private void UpdateAvailable()
    {
        SetProgress(0, $"Update available! (v{Utils.GetVerString()} -> {Release.TagName})");
        SetTitleText("Update available!");

        string authorLink = string.IsNullOrEmpty(Release.Author.HtmlUrl)
            ? Release.Author.Login
            : $"<a href=\"{Release.Author.HtmlUrl}\">{Release.Author.Login}</a>";

        // show the changelog of the latest release from GitHub
        wbChangelog.DocumentText = GetHtml(Strings.GetString("Changelog",
            Release.Name,
            Release.PreRelease ? Strings.GetString("PreReleaseTag") : string.Empty,
            $"{Release.PublishedAt.ToLocalTime():g}",
            authorLink, Release.HtmlUrl, Markdown.Parse(Release.Body)), true);

        btnUpdate.Text = $"&Update to {Release.TagName}";
        btnUpdate.Tag = "update";
        if (AutoUpdate)
        {
            btnLater.Enabled = btnLater.Visible = true;
            btnDisable.Enabled = btnDisable.Visible = true;
        }
    }

    private void UpdateError(Exception ex, string shortMsg, string longMsg)
    {
        SetProgress(0, $"ERROR: {shortMsg}: {(ex.InnerException is WebException ex2 ? ex2.Message : ex.Message)}");
        wbChangelog.DocumentText = GetHtml(Strings.GetString(
            longMsg, ex));
        btnUpdate.Text = "Retry update";
        // re-enable update button to allow retry
        btnUpdate.Enabled = true;
    }

    private void DownloadProgress(long bytesReceived, long fileSize)
    {
        SetProgress((int)(bytesReceived * 100 / fileSize),
            $"Downloading update ({FormatByteCount(fileSize - bytesReceived)} remaining)...");
    }

    private static async Task ExtractUpdate(string src, string dest)
    {
        // delete target directory if it exists
        try
        {
            Directory.Delete(dest, true);
        }
        catch (DirectoryNotFoundException) { }

        // extract the archive
        Directory.CreateDirectory(dest);
        await Task.Run(() => ZipFile.ExtractToDirectory(src, dest));
        return;
    }

    private void InstallUpdate()
    {
        try
        {
            // run the installer from a different location so we can
            // clean the old directory
            Utils.RunCmd(Path.Combine(OldPath, ExeName),
                $"--install {OldPath} {UpdatePath} {TargetPath}", false);
            Close();
        }
        catch (Win32Exception ex)
        {
            if (ex.ErrorCode == -2147467259) // 0x80004005 - operation cancelled by user
            {
                SetProgress(100, Strings.GetString("InstallPrompt"));
                Utils.ShowError("Admin is required to finish update install.");
                btnUpdate.Tag = "install";
                btnUpdate.Text = "Install update";
                btnUpdate.Enabled = true;
                return;
            }
            throw;
        }
    }

    private void SetTitleText(string title)
    {
        Text = string.IsNullOrEmpty(title)
            ? "YAMDCC updater"
            : $"{title} - YAMDCC updater";
        Text += Utils.IsAdmin() ? " (Administrator)" : string.Empty;
    }

    private static string GetHtml(string text, bool html = false)
    {
        return Strings.GetString("HtmlTemplate", html ? text : Markdown.Parse(text));
    }

    private void SetProgress(int progress, string text)
    {
        if (progress < 0)
        {
            pbProgress.Style = ProgressBarStyle.Marquee;
        }
        else
        {
            pbProgress.Style = ProgressBarStyle.Blocks;
            pbProgress.Value = progress;
        }

        if (!string.IsNullOrEmpty(text))
        {
            grpProgress.Text = text;
        }
    }

    internal static void CopyDir(DirectoryInfo src, DirectoryInfo dest)
    {
        Directory.CreateDirectory(dest.FullName);

        // Copy each file into the new directory.
        foreach (FileInfo fi in src.GetFiles())
        {
            if (fi.FullName != DownloadPath)
            {
                fi.CopyTo(Path.Combine(dest.FullName, fi.Name), true);
            }
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in src.GetDirectories())
        {
            if (diSourceSubDir.FullName != OldPath &&
                diSourceSubDir.FullName != UpdatePath)
            {
                DirectoryInfo nextTargetSubDir =
                dest.CreateSubdirectory(diSourceSubDir.Name);
                CopyDir(diSourceSubDir, nextTargetSubDir);
            }
        }
    }

    private static string FormatByteCount(long bytes)
    {
        int mult = 1024;
        char[] chars = ['K', 'M', 'G'];

        for (int i = 2; i >= 0; i--)
        {
            double unit = Math.Pow(mult, i + 1);
            if (bytes > unit)
            {
                return $"{Math.Round(bytes / unit, 2)} {chars[i]}B";
            }
        }
        return $"{bytes} bytes";
    }

    private static bool SetAutoUpdate(bool enabled)
    {
        if (!Utils.IsAdmin())
        {
            try
            {
                return Utils.RunCmd(Assembly.GetExecutingAssembly().Location, $"--setautoupdate {enabled}") == 0;
            }
            catch (Win32Exception ex)
            {
                if (ex.ErrorCode == -2147467259) // 0x80004005 - operation cancelled by user
                {
                    Utils.ShowError("Admin is required to change auto-update setting.");
                    return false;
                }
                throw;
            }
        }
        else
        {
            Updater.SetAutoUpdateEnabled(false);
            return true;
        }
    }
}
