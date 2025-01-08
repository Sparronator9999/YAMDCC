using Microsoft.Win32.TaskScheduler;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using YAMDCC.Common;

namespace YAMDCC.Updater;

public static class Updater
{
    private const string UpdateTask = "YAMDCC auto-update task";

    private static readonly string BuildConfig = Assembly.GetCallingAssembly()
        .GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration;

    private static readonly GitHubClient GHClient = new(
        new ProductHeaderValue("YAMDCC.Updater"));

    /// <summary>
    /// Gets the latest YAMDCC GitHub release.
    /// </summary>
    /// <param name="preRelease">
    /// Set to <c>true</c> to include pre-release updates.
    /// </param>
    /// <returns>
    /// The latest YAMDCC release (or pre-release if
    /// <paramref name="preRelease"/> was <c>true</c>),
    /// or <c>null</c> if no release was found.
    /// </returns>
    /// <exception cref="HttpRequestException"/>
    /// <exception cref="ApiException"/>
    public static async Task<Release> GetLatestReleaseAsync(bool preRelease)
    {
        IReadOnlyList<Release> releases = await GHClient.Repository.Release.GetAll(
            Paths.ProjectRepo.Split('/')[0], Paths.ProjectRepo.Split('/')[1],
            new ApiOptions { PageSize = 20, PageCount = 1 });

        for (int i = 0; i < releases.Count; i++)
        {
            if (releases[i].Prerelease && !preRelease)
            {
                continue;
            }
            return releases[i];
        }

        return null;
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
    /// An optional callback for asset download progress updates.
    /// </param>
    /// <param name="complete">
    /// An optional callback for when the asset download is complete.
    /// </param>
    /// <returns>
    /// <c>true</c> if the download was started successfully,
    /// <c>false</c> if no release assets match the updater's build configuration,
    /// or the browser download URL was null/empty.
    /// </returns>
    /// <exception cref="ArgumentException"/>
    public static bool DownloadUpdateAsync(
        Release release, string path,
        Action<object, DownloadProgressChangedEventArgs> progress,
        Action<object, AsyncCompletedEventArgs> complete)
    {
        if (release is null)
        {
            throw new ArgumentNullException(nameof(release));
        }
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }
        if (progress is null)
        {
            throw new ArgumentNullException(nameof(release));
        }
        if (complete is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        // download the first YAMDCC version that
        // matches the current build configuration
        string url = release.Assets.First((asset) =>
            asset.Name.Contains(BuildConfig)).BrowserDownloadUrl;

        using (WebClient client = new())
        {
            client.DownloadProgressChanged += progress.Invoke;
            client.DownloadFileCompleted += complete.Invoke;
            client.DownloadFileAsync(new Uri(url), path);
        }
        return true;
    }

    public static bool GetAutoUpdateEnabled()
    {
        return TaskService.Instance.GetTask(UpdateTask) is not null;
    }

    public static void SetAutoUpdateEnabled(bool enable)
    {
        TaskService ts = TaskService.Instance;
        if (enable)
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
}
