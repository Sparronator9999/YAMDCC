using Markdig;
using Octokit;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;
using YAMDCC.Common;

namespace YAMDCC.Updater;

internal partial class UpdateForm : Form
{
    private readonly bool AutoUpdate;

    private Release[] Releases;

    private readonly BackgroundWorker ExtractWorker = new();

    private static readonly string ExeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
    private static readonly string TargetPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    private static readonly string DownloadPath = Path.GetFullPath("YAMDCC-Update.zip");
    private static readonly string UpdatePath = Path.GetFullPath("Update");
    private static readonly string OldPath = Path.GetFullPath("Old");

    private static readonly string[] ExtractArgs = [DownloadPath, UpdatePath];

    public UpdateForm(Release[] releases = null, bool autoUpdate = false)
    {
        InitializeComponent();
        Icon = Utils.GetEntryAssemblyIcon();
        AutoUpdate = autoUpdate;

        SetTitleText(string.Empty);
        tsiPreRelease.Checked = Utils.GetCurrentVerSuffix() != "release";
        tsiAutoUpdate.Checked = Updater.GetAutoUpdateEnabled();

        if (releases is null)
        {
            wbChangelog.DocumentText = GetHtml(
                Resources.GetString("UpdatePrompt") +
                (tsiPreRelease.Checked ? Resources.GetString("PreReleaseOn") : string.Empty));
        }
        else
        {
            Releases = releases;
            UpdateAvailable();
        }
    }

    private void wbChangelog_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        string url = e.Url.ToString();

        if (url.StartsWith("yamdcc:", StringComparison.OrdinalIgnoreCase))
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
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            Process.Start(url);
            e.Cancel = true;
        }
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        if ("update".Equals(btnUpdate.Tag) &&
            Releases is not null && Releases.Length > 0)
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

            Updater.DownloadUpdateAsync(Releases[0], DownloadPath,
                DownloadProgress, DownloadComplete);
            return;
        }
        else
        {
            CheckUpdate();
        }
    }

    private void btnRemindLater_Click(object sender, EventArgs e)
    {
        // just close the updater and let Windows task
        // scheduler do the reminding by re-running the updater
        Close();
    }

    private void btnDisableUpdates_Click(object sender, EventArgs e)
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
        if (!"update".Equals(btnUpdate.Tag))
        {
            wbChangelog.DocumentText = GetHtml(
                Resources.GetString("UpdatePrompt") +
                (tsiPreRelease.Checked ? Resources.GetString("PreReleaseOn") : string.Empty));
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
            Releases = await Updater.GetReleasesAsync(tsiPreRelease.Checked);
        }
        catch (Exception ex)
        {
            if (ex is HttpRequestException or ApiException)
            {
                SetProgress(0, $"ERROR: {(ex.InnerException is WebException ex2 ? ex2.Message : ex.Message)}");
                wbChangelog.DocumentText = GetHtml(Markdown.ToHtml(Resources.GetString(
                    "errCheckUpdate", ex)));
                btnUpdate.Enabled = true;
                btnOptions.Enabled = true;
                return;
            }
            else
            {
                throw;
            }
        }

        if (Releases is null || Releases.Length == 0)
        {
            SetProgress(0, Resources.GetString("errNoReleaseS"));
            wbChangelog.DocumentText = GetHtml(Resources.GetString(
                "errNoRelease"));
        }
        else
        {
            Version current = Utils.GetCurrentVersion(),
                latest = Utils.GetVersion(Releases[0].TagName.Remove(0, 1));

            if (current == latest)
            {
                SetProgress(100, "YAMDCC is up-to-date.");
                SetTitleText("Up to date");
                wbChangelog.DocumentText = GetHtml("YAMDCC is up-to-date.\n\n" +
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
        Release latest = Releases[0];
        SetProgress(0, $"Update available! (v{Utils.GetVerString()} -> {latest.TagName})");
        SetTitleText("Update available!");

        string authorLink = string.IsNullOrEmpty(Releases[0].Author.HtmlUrl)
            ? latest.Author.Login
            : $"<a href=\"{latest.Author.HtmlUrl}\">{latest.Author.Login}</a>";

        // show the changelog of the latest release from GitHub
        wbChangelog.DocumentText = GetHtml(Resources.GetString("Changelog",
            latest.Name,
            latest.Prerelease ? Resources.GetString("PreReleaseTag") : string.Empty,
            $"{latest.PublishedAt.Value.ToLocalTime():g}",
            authorLink, latest.HtmlUrl, Markdown.ToHtml(latest.Body)), true);

        btnUpdate.Text = $"&Update to {latest.TagName}";
        btnUpdate.Tag = "update";
        if (AutoUpdate)
        {
            btnLater.Enabled = btnLater.Visible = true;
            btnDisable.Enabled = btnDisable.Visible = true;
        }
    }

    private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        SetProgress(e.ProgressPercentage, $"Downloading update ({FormatByteCount(e.TotalBytesToReceive - e.BytesReceived)} remaining)...");
    }

    private void DownloadComplete(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error is null)
        {
            SetProgress(-1, "Extracting update...");
            ExtractWorker.DoWork += ExtractUpdate;
            ExtractWorker.RunWorkerCompleted += ExtractComplete;
            ExtractWorker.RunWorkerAsync(ExtractArgs);
        }
        else
        {
            // re-enable update button to allow retry
            btnUpdate.Enabled = true;
            btnUpdate.Text = "Retry update";
            SetProgress(0, $"ERROR: Failed to download YAMDCC: {e.Error.Message}");
        }
    }

    private void ExtractUpdate(object sender, DoWorkEventArgs e)
    {
        // args: archivePath, destPath
        if (e.Argument is string[] args && args.Length >= 2)
        {
            // delete target directory if it exists
            try
            {
                Directory.Delete(args[1], true);
            }
            catch (DirectoryNotFoundException) { }

            // extract the archive
            Directory.CreateDirectory(args[1]);
            ZipFile.ExtractToDirectory(args[0], args[1]);
            return;
        }
    }

    private void ExtractComplete(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Error is null)
        {
            // install the update
            SetProgress(-1, "Preparing to install update...");

            // make a copy of the old YAMDCC installation
            try
            {
                Directory.Delete(OldPath, true);
            }
            catch (DirectoryNotFoundException) { }
            CopyDirectory(TargetPath, OldPath);

            // run the installer from a different location so we can
            // clean the old directory
            Utils.RunCmd(Path.Combine(OldPath, ExeName),
                $"--install {OldPath} {UpdatePath} {TargetPath}", false);
            Close();
        }
        else
        {
            // re-enable update button to allow retry
            btnUpdate.Enabled = true;
            btnUpdate.Text = "Retry update";
            SetProgress(0, $"ERROR: Failed to extract update: {e.Error.Message}");
        }
    }

    private void SetTitleText(string title)
    {
        Text = string.IsNullOrEmpty(title)
            ? "YAMDCC updater"
            : $"{title} - YAMDCC updater";
        Text += Utils.IsAdmin() ? " (Administrator)" : string.Empty;
    }

    private static string GetHtml(string markdown, bool html = false)
    {
        return Resources.GetString("HtmlTemplate", html ? markdown : Markdown.ToHtml(markdown));
    }

    private void SetProgress(int progress, string message)
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

        if (!string.IsNullOrEmpty(message))
        {
            grpProgress.Text = message;
        }
    }

    internal static void CopyDirectory(string sourceDir, string destDir)
    {
        DirectoryInfo diSource = new(sourceDir);
        DirectoryInfo diTarget = new(destDir);

        CopyDir(diSource, diTarget);
    }

    internal static void CopyDir(DirectoryInfo source, DirectoryInfo dest)
    {
        Directory.CreateDirectory(dest.FullName);

        // Copy each file into the new directory.
        foreach (FileInfo fi in source.GetFiles())
        {
            if (fi.FullName != DownloadPath)
            {
                fi.CopyTo(Path.Combine(dest.FullName, fi.Name), true);
            }
        }

        // Copy each subdirectory using recursion.
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
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
                else
                {
                    throw;
                }
            }
        }
        else
        {
            Updater.SetAutoUpdateEnabled(false);
            return true;
        }
    }
}
