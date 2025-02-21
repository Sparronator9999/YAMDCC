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
using System;

namespace YAMDCC.Updater.GitHubApi;

// suppress warning about default values never getting overwritten
// since they get populated when deserialising JSON to these classes
#pragma warning disable CS0649
internal sealed class Release
{
    [JsonProperty("url")]
    public string Url;

    [JsonProperty("html_url")]
    public string HtmlUrl;

    [JsonProperty("assets_url")]
    public string AssetsUrl;

    [JsonProperty("upload_url")]
    public string UploadUrl;

    [JsonProperty("tarball_url")]
    public string TarballUrl;

    [JsonProperty("zipball_url")]
    public string ZipballUrl;

    [JsonProperty("id")]
    public int Id;

    [JsonProperty("node_id")]
    public string NodeId;

    [JsonProperty("tag_name")]
    public string TagName;

    [JsonProperty("target_commitish")]
    public string TargetCommitish;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("body")]
    public string Body;

    [JsonProperty("draft")]
    public bool Draft;

    [JsonProperty("prerelease")]
    public bool PreRelease;

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt;

    [JsonProperty("published_at")]
    public DateTimeOffset PublishedAt;

    [JsonProperty("author")]
    public Author Author;

    [JsonProperty("assets")]
    public ReleaseAsset[] Assets;
}
#pragma warning restore CS0649
