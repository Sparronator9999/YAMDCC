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

    private Release LatestRelease;

    private readonly BackgroundWorker ExtractWorker = new();

    private static readonly string ExeName = Path.GetFileName(Assembly.GetEntryAssembly().Location);
    private static readonly string TargetPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    private static readonly string DownloadPath = Path.GetFullPath("YAMDCC-Update.zip");
    private static readonly string UpdatePath = Path.GetFullPath("Update");
    private static readonly string OldPath = Path.GetFullPath("Old");

    private static readonly string[] ExtractArgs = [DownloadPath, UpdatePath];

    public UpdateForm(Release release = null, bool autoUpdate = false)
    {
        InitializeComponent();
        Icon = Utils.GetEntryAssemblyIcon();
        AutoUpdate = autoUpdate;

        SetTitleText(string.Empty);
        tsiPreRelease.Checked = Utils.GetVerSuffix() != "release";
        tsiAutoUpdate.Checked = Updater.GetAutoUpdateEnabled();

        if (release is null)
        {
            wbChangelog.DocumentText = GetHtml(
                Resources.GetString("UpdatePrompt") +
                (tsiPreRelease.Checked ? Resources.GetString("PreReleaseOn") : string.Empty));
        }
        else
        {
            LatestRelease = release;
            UpdateAvailable();
        }
    }

    private void wbChangelog_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
        string url = e.Url.ToString();

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
        if (LatestRelease is null)
        {
            if (Utils.GetVerSuffix() != "dev" ||
                Utils.ShowWarning(Resources.GetString("warnDev"),
                "Dev version detected!") == DialogResult.Yes)
            {
                CheckUpdate();
            }
            return;
        }

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

        Updater.DownloadUpdateAsync(LatestRelease, DownloadPath,
            DownloadProgress, DownloadComplete);
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
        if (!Utils.IsAdmin())
        {
            Utils.RunCmd(Assembly.GetExecutingAssembly().Location, $"--setautoupdate {false}");
        }
        else
        {
            Updater.SetAutoUpdateEnabled(false);
        }
        Close();
    }

    private void btnOptions_Click(object sender, EventArgs e)
    {
        OptMenu.Show(MousePosition);
    }

    private void tsiAutoUpdate_Click(object sender, EventArgs e)
    {
        if (!Utils.IsAdmin())
        {
            if (Utils.RunCmd(Assembly.GetExecutingAssembly().Location, $"--setautoupdate {!tsiAutoUpdate.Checked}") == 0)
            {
                tsiAutoUpdate.Checked = !tsiAutoUpdate.Checked;
            }
            return;
        }
        else
        {
            tsiAutoUpdate.Checked = !tsiAutoUpdate.Checked;
            Updater.SetAutoUpdateEnabled(tsiAutoUpdate.Checked);
        }
    }

    private void tsiPreRelease_Click(object sender, EventArgs e)
    {
        if (LatestRelease is null)
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
            LatestRelease = await Updater.GetLatestReleaseAsync(tsiPreRelease.Checked);
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

        if (LatestRelease is null)
        {
            SetProgress(0, Resources.GetString("errNoReleaseS"));
            wbChangelog.DocumentText = GetHtml(Resources.GetString(
                "errNoRelease"));

        }
        else if ($"v{Utils.GetVerString()}" == LatestRelease.TagName)
        {
            LatestRelease = null;
            SetProgress(100, "YAMDCC is up-to-date.");
            SetTitleText("Up to date");
            wbChangelog.DocumentText = GetHtml("YAMDCC is up-to-date.");
        }
        else
        {
            UpdateAvailable();
        }
        btnUpdate.Enabled = true;
        btnOptions.Enabled = true;
    }

    private void UpdateAvailable()
    {
        SetProgress(0, $"Update available! (v{Utils.GetVerString()} -> {LatestRelease.TagName})");
        SetTitleText("Update available!");

        // show the changelog of the latest release from GitHub
        string authorLink = string.IsNullOrEmpty(LatestRelease.Author.HtmlUrl)
            ? LatestRelease.Author.Login
            : $"<a href=\"{LatestRelease.Author.HtmlUrl}\">{LatestRelease.Author.Login}</a>";
        wbChangelog.DocumentText = GetHtml(Resources.GetString("Changelog",
            LatestRelease.Name,
            LatestRelease.Prerelease ? Resources.GetString("PreReleaseTag") : string.Empty,
            $"{LatestRelease.PublishedAt.Value.ToLocalTime():g}",
            authorLink, LatestRelease.HtmlUrl, Markdown.ToHtml(LatestRelease.Body)), true);

        btnUpdate.Text = $"&Update to {LatestRelease.TagName}";
        if (AutoUpdate)
        {
            btnLater.Enabled = btnLater.Visible = true;
            btnDisable.Enabled = btnDisable.Visible = true;
        }
    }

    private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        SetProgress(e.ProgressPercentage, $"Downloading update ({FormatByteCount(e.TotalBytesToReceive - e.BytesReceived)} bytes remaining)...");
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

    // TODO: test
    private static string FormatByteCount(long bytes)
    {
        int mult = 1024;
        char[] chars = ['K', 'M', 'G'];

        for (int i = 2; i >= 0; i--)
        {
            int unit = (int)Math.Pow(mult, i + 1);
            if (bytes > unit)
            {
                return $"{bytes / unit} {chars[i]}B";
            }
        }
        return $"{bytes} bytes";
    }
}
