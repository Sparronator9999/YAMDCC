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

using Newtonsoft.Json;

namespace YAMDCC.Updater.GitHubApi;

// suppress warning about default values never getting overwritten
// since they get populated when deserialising JSON to these classes
#pragma warning disable CS0649
internal sealed class ApiError
{
    [JsonProperty("message")]
    public string Message;

    [JsonProperty("documentation_url")]
    public string DocsUrl;

    [JsonProperty("status")]
    public string Status;
}
#pragma warning restore CS0649
