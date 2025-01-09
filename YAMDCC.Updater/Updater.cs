using Microsoft.Win32.TaskScheduler;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using YAMDCC.Common;

namespace YAMDCC.Updater;

public static class Updater
{
    private const string UpdateTask = "YAMDCC auto-update task";

    private static readonly string BuildConfig = Assembly.GetCallingAssembly()
        .GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration;

    private static readonly ProductHeaderValue ProductHeader =
        new("YAMDCC.Updater", Utils.GetVerString());

    private static readonly GitHubClient GHClient = new(ProductHeader);

    /// <summary>
    /// Gets a set of the most recent YAMDCC releases.
    /// </summary>
    /// <param name="preRelease">
    /// Set to <see langword="true"/> to include pre-release updates.
    /// </param>
    /// <param name="numReleases">
    /// The number of releases to return. Defaults to the 20 most recent releases.
    /// </param>
    /// <returns>
    /// A <see cref="Release"/> array with up to <paramref name="numReleases"/>
    /// most recent releases.
    /// </returns>
    public static async Task<Release[]> GetReleasesAsync(bool preRelease, int numReleases = 20)
    {
        IReadOnlyList<Release> releases = await GHClient.Repository.Release.GetAll(
            Paths.ProjectRepo.Split('/')[0], Paths.ProjectRepo.Split('/')[1],
            new ApiOptions { PageSize = numReleases, PageCount = 1 });

        return preRelease
            ? [.. releases]
            : releases.Where((rel) => !rel.Prerelease).ToArray();
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
            asset.Name.Contains(BuildConfig)).Url;

        using (WebClient client = new())
        {
            client.Headers.Add(HttpRequestHeader.UserAgent, ProductHeader.ToString());
            client.Headers.Add(HttpRequestHeader.Accept, "application/octet-stream");
            if (GHClient.Credentials.AuthenticationType != AuthenticationType.Anonymous)
            {
                client.Headers.Add(HttpRequestHeader.Authorization, $"token {GHClient.Credentials.Password}");
            }
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
