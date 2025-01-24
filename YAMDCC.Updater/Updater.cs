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

using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using YAMDCC.Common;
using YAMDCC.Updater.GitHubApi;
using Task = System.Threading.Tasks.Task;

namespace YAMDCC.Updater;

internal static class Updater
{
    private const string UpdateTask = "YAMDCC auto-update task";

    private static readonly string BuildConfig = Assembly.GetCallingAssembly()
        .GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration;

    public static bool IsDevVersion(Release release)
    {
        return Utils.GetCurrentVersion() > Utils.GetVersion(release.TagName.Remove(0, 1)) ||
            Utils.GetCurrentVerSuffix() == "dev";
    }

    public static bool IsUpdateAvailable(Release release)
    {
        string tag = release.TagName.Remove(0, 1),
            currentSuffix = Utils.GetCurrentVerSuffix(),
            latestSuffix = Utils.GetVerSuffix(tag);

        Version currentVer = Utils.GetCurrentVersion(),
            latestVer = Utils.GetVersion(tag);

        // check if version suffixes are different,
        // if they are, we probably need to update
        if (latestSuffix != string.Empty && latestSuffix != currentSuffix ||
            latestSuffix == string.Empty && currentSuffix != "release")
        {
            return true;
        }

        if (currentVer == latestVer)
        {
            // check if pre-release version is out of date
            int i = 0, currentSuffixVer, latestSuffixVer;
            do
            {
                currentSuffixVer = Utils.GetCurrentSuffixVer(i);
                latestSuffixVer = Utils.GetSuffixVer(tag, i);

                // this works even if current version
                // doesn't have extra suffixes :)
                if (currentSuffixVer < latestSuffixVer)
                {
                    return true;
                }
            }
            while (currentSuffixVer != -1 || latestSuffixVer != -1);

            // YAMDCC is up to date if we pass all these checks
            return false;
        }
        return true;
    }

    /// <summary>
    /// Gets the latest YAMDCC release
    /// (or pre-release if <paramref name="preRelease"/> is <see langword="true"/>).
    /// </summary>
    /// <param name="preRelease">
    /// Set to <see langword="true"/> to get the latest pre-release.
    /// </param>
    /// <returns>
    /// The latest <see cref="Release"/> (if it exists),
    /// otherwise <see langword="null"/>.
    /// </returns>
    public static async Task<Release> GetLatestReleaseAsync(bool preRelease)
    {
        using (HttpClient client = new())
        {
            client.BaseAddress = new Uri(Paths.GitHubApiUrl);
            client.DefaultRequestHeaders.Add("User-Agent", $"YAMDCC.Updater/{Utils.GetVerString()}");

            // get latest release directly if we want non-prerelease,
            // otherwise get latest few releases in case the latest is a draft release
            using (HttpResponseMessage response = await client.GetAsync(
                $"repos/{Paths.ProjectRepo}/releases{(preRelease ? "?per_page=5" : "/latest")}"))
            {
                try
                {
                    response.ThrowOnApiError();
                }
                catch (HttpRequestException)
                {
                    // 404 is returned by API if there is no latest release,
                    // so just return null to match pre-release behaviour
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    throw;
                }

                string respBody = await response.Content.ReadAsStringAsync();
                if (preRelease)
                {
                    Release[] releases = JsonConvert.DeserializeObject<Release[]>(respBody);
                    return releases.FirstOrDefault((release) => !release.Draft);
                }
                else
                {
                    return JsonConvert.DeserializeObject<Release>(respBody);
                }
            }
        }
    }

    /// <summary>
    /// Downloads the first release asset for the specified
    /// <paramref name="release"/> that matches the updater's
    /// build configuration.
    /// </summary>
    /// <param name="release">
    /// The <see cref="Release"/> to download the asset from.
    /// </param>
    /// <param name="path">
    /// Where to save the downloaded release asset.
    /// </param>
    /// <param name="progress">
    /// <para>An optional callback for asset download progress updates.</para>
    /// <para>Parameters:<br/>
    /// - bytesReceived: The number of bytes received so far.<br/>
    /// - totalBytesToReceive: The total size of the release asset being downloaded, or -1 if it couldn't be determined.
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the download was started successfully,
    /// <see langword="false"/> if no release assets match the updater's build
    /// configuration, or the browser download URL was <see langword="null"/>/empty.
    /// </returns>
    /// <exception cref="ArgumentException"/>
    public static async Task DownloadUpdateAsync(
        Release release, string path,
        Action<long, long> progress)
    {
        if (release is null)
        {
            throw new ArgumentNullException(nameof(release));
        }
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        // download the first YAMDCC version that
        // matches the current build configuration
        string url = release.Assets.FirstOrDefault((asset) =>
            asset.Name.Contains(BuildConfig))?.Url;

        using (HttpClient client = new())
        {
            client.DefaultRequestHeaders.Add("User-Agent", $"YAMDCC.Updater/{Utils.GetVerString()}");
            client.DefaultRequestHeaders.Add("Accept", "application/octet-stream");

            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.ThrowOnApiError();

                // partially based on: https://stackoverflow.com/q/20661652
                using (Stream src = await response.Content.ReadAsStreamAsync())
                using (FileStream dest = File.Create(path, 8192))
                {
                    byte[] buf = new byte[8192];
                    long fileSize = response.Content.Headers.ContentLength ?? -1,
                        totalBytes = 0;

                    while (true)
                    {
                        int bytesRead = await src.ReadAsync(buf, 0, buf.Length);
                        totalBytes += bytesRead;

                        if (bytesRead == 0)
                        {
                            break;
                        }

                        await dest.WriteAsync(buf, 0, bytesRead);
                        progress?.Invoke(totalBytes, fileSize);
                    }
                }
            }
        }
    }

    public static bool GetAutoUpdateEnabled()
    {
        return TaskService.Instance.GetTask(UpdateTask) is not null;
    }

    public static void SetAutoUpdateEnabled(bool enabled)
    {
        TaskService ts = TaskService.Instance;
        if (enabled)
        {
            // create a new task if it doesn't exist already;
            // otherwise use the existing one
            TaskDefinition td = (ts.GetTask(UpdateTask)?.Definition) ?? ts.NewTask();

            td.RegistrationInfo.Description = "Checks for new YAMDCC updates automatically on logon.";
            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Principal.RunLevel = TaskRunLevel.Highest;

            // check for updates at login, then repeat once a day,
            // every day, f o r e v e r .
            LogonTrigger lt = td.Triggers.Add(new LogonTrigger());
            lt.Delay = TimeSpan.FromSeconds(30);
            lt.Repetition.Duration = TimeSpan.Zero;
            lt.Repetition.Interval = TimeSpan.FromDays(1);

            // add this executable (Updater.exe) to list of actions
            td.Actions.Add(Assembly.GetExecutingAssembly().Location, "--autoupdate");

            // we (obviously) need internet to check for updates
            td.Settings.RunOnlyIfNetworkAvailable = true;

            // if update check fails, try again in an hour up to 3 times,
            // otherwise wait until the next login/day
            td.Settings.RestartInterval = TimeSpan.FromHours(1);
            td.Settings.RestartCount = 3;

            // allow running even on battery power
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.StopIfGoingOnBatteries = false;

            // actually register the task
            ts.RootFolder.RegisterTaskDefinition(UpdateTask, td);
        }
        else
        {
            // try to delete task if it exists
            ts.RootFolder.DeleteTask(UpdateTask, false);
        }
    }

    private static void ThrowOnApiError(this HttpResponseMessage response)
    {
        string body = string.Empty;
        try
        {
            body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                case (HttpStatusCode)429:   // Too Many Requests
                    ApiError error = JsonConvert.DeserializeObject<ApiError>(body);
                    throw new HttpRequestException($"API error: {error.Message}", ex);
                default:
                    throw;
            }
        }
    }
}
