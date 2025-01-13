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
internal class Author
{
    [JsonProperty("login")]
    public string Login;

    [JsonProperty("id")]
    public int Id;

    [JsonProperty("node_id")]
    public string NodeId;

    [JsonProperty("avatar_url")]
    public string AvatarUrl;

    [JsonProperty("gravatar_id")]
    public string GravatarId;

    [JsonProperty("url")]
    public string Url;

    [JsonProperty("html_url")]
    public string HtmlUrl;

    [JsonProperty("followers_url")]
    public string FollowersUrl;

    [JsonProperty("following_url")]
    public string FollowingUrl;

    [JsonProperty("gists_url")]
    public string GistsUrl;

    [JsonProperty("starred_url")]
    public string StarredUrl;

    [JsonProperty("subscriptions_url")]
    public string SubsUrl;

    [JsonProperty("organizations_url")]
    public string OrganizationsUrl;

    [JsonProperty("repos_url")]
    public string ReposUrl;

    [JsonProperty("events_url")]
    public string EventsUrl;

    [JsonProperty("received_events_url")]
    public string ReceivedEventsUrl;

    [JsonProperty("type")]
    public string Type;

    [JsonProperty("site_admin")]
    public bool SiteAdmin;
}
#pragma warning restore CS0649
