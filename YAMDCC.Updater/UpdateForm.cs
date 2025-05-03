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
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Configs;
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

    private static readonly string TempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private static readonly string DownloadPath = Path.Combine(TempPath, "YAMDCC-Update.exe");

    public UpdateForm(Release release = null, bool autoUpdate = false)
    {
        InitializeComponent();
        Icon = Utils.GetEntryAssemblyIcon();
        AutoUpdate = autoUpdate;

        SetTitleText(string.Empty);
        tsiPreRelease.Checked = CommonConfig.GetPreRelease();
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

            // download the update installer EXE
            SetProgress(-1, "Downloading installer...");
            Directory.CreateDirectory(TempPath);
            try
            {
                await Updater.DownloadUpdateAsync(
                    Release, DownloadPath, DownloadProgress);
            }
            catch (HttpRequestException ex)
            {
                UpdateError(ex, "failed to download installer", "errDownload");
            }

            // install the update
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
        SetAdminSetting("setautoupdate", false);
        Close();
    }

    private void btnOptions_Click(object sender, EventArgs e)
    {
        OptMenu.Show(MousePosition);
    }

    private void tsiAutoUpdate_Click(object sender, EventArgs e)
    {
        if (SetAdminSetting("setautoupdate", !tsiAutoUpdate.Checked))
        {
            tsiAutoUpdate.Checked = !tsiAutoUpdate.Checked;
        }
    }

    private void tsiPreRelease_Click(object sender, EventArgs e)
    {
        if (SetAdminSetting("setprerelease", !tsiPreRelease.Checked))
        {
            tsiPreRelease.Checked = !tsiPreRelease.Checked;
            if (btnUpdate.Tag is null)
            {
                wbChangelog.DocumentText = GetHtml(
                    Strings.GetString("UpdatePrompt") +
                    (tsiPreRelease.Checked ? Strings.GetString("PreReleaseOn") : string.Empty));
            }
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
            if (Release is null && !tsiPreRelease.Checked)
            {
                // there's no non-prerelease version yet, try to get latest pre-release
                Release = await Updater.GetLatestReleaseAsync(true);
            }
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
            if (Updater.IsDevVersion(Release))
            {
                SetProgress(100, "Current YAMDCC version > Latest YAMDCC version???");
                SetTitleText("Dev version detected");
                wbChangelog.DocumentText = GetHtml(Strings.GetString("DevVer"));
            }
            else if (Updater.IsUpdateAvailable(Release))
            {
                UpdateAvailable();
            }
            else
            {
                SetProgress(100, "YAMDCC is up to date.");
                SetTitleText("Up to date");
                wbChangelog.DocumentText = GetHtml(Strings.GetString("UpToDate"));
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

    private void UpdateError(HttpRequestException ex, string shortMsg, string longMsg)
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
            $"Downloading installer ({FormatByteCount(fileSize - bytesReceived)} remaining)...");
    }

    private void InstallUpdate()
    {
        try
        {
            // run the installer from a different location so we can
            // clean the old directory
            Utils.RunCmd(DownloadPath, "/SP- /silent /noicons", false);
            Close();
        }
        catch (Win32Exception ex)
        {
            if (ex.ErrorCode == -2147467259) // 0x80004005 - operation cancelled by user
            {
                SetProgress(100, Strings.GetString("InstallPrompt"));
                Utils.ShowError(Strings.GetString("dlgAdminInstall"));
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

    private static bool SetAdminSetting(string cmd, bool enabled)
    {
        try
        {
            return Utils.RunCmd(Assembly.GetExecutingAssembly().Location, $"--{cmd} {enabled}") == 0;
        }
        catch (Win32Exception ex)
        {
            if (ex.ErrorCode == -2147467259) // 0x80004005 - operation cancelled by user
            {
                Utils.ShowError(Strings.GetString("dlgAdminSetting"));
                return false;
            }
            throw;
        }
    }
}
