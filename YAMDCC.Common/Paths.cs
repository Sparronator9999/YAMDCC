// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2023-2025.
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
using System.IO;

namespace YAMDCC.Common;

public static class Paths
{
    /// <summary>
    /// The GitHub home page.
    /// </summary>
    public static readonly string GitHubUrl = "https://github.com";

    /// <summary>
    /// The project repository.
    /// </summary>
    public static readonly string ProjectRepo = "Sparronator9999/YAMDCC";

    /// <summary>
    /// The URL to this project's GitHub page.
    /// </summary>
    public static readonly string GitHubPage = $"{GitHubUrl}/{ProjectRepo}";

    /// <summary>
    /// The path where program data is stored.
    /// </summary>
    /// <remarks>
    /// (C:\ProgramData\Sparronator9999\YAMDCC on Windows)
    /// </remarks>
    public static readonly string Data = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        ProjectRepo.Split('/')[0], ProjectRepo.Split('/')[1]);

    /// <summary>
    /// The path where YAMDCC service logs are saved.
    /// </summary>
    /// <remarks>
    /// (C:\ProgramData\Sparronator9999\YAMDCC\Logs on Windows)
    /// </remarks>
    public static readonly string Logs = Path.Combine(Data, "Logs");

    /// <summary>
    /// The path where the config containing common settings
    /// across all YAMDCC applications is saved.
    /// </summary>
    /// <remarks>
    /// (C:\ProgramData\Sparronator9999\YAMDCC\GlobalConfig.xml on Windows)
    /// </remarks>
    public static readonly string GlobalConf = Path.Combine(Data, "GlobalConfig.xml");

    /// <summary>
    /// The path where the currently applied YAMDCC config is saved.
    /// </summary>
    /// <remarks>
    /// (C:\ProgramData\Sparronator9999\YAMDCC\CurrentConfig.xml on Windows)
    /// </remarks>
    public static readonly string CurrentConf = Path.Combine(Data, "CurrentConfig.xml");
}
