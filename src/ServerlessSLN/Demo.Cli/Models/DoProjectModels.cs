using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Demo.Cli.Models
{
    public class Project
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("state")] public string State { get; set; }

        [JsonProperty("revision")] public int Revision { get; set; }

        [JsonProperty("visibility")] public string Visibility { get; set; }

        [JsonProperty("lastUpdateTime")] public DateTime LastUpdateTime { get; set; }
    }

    public class Projects
    {
        [JsonProperty("count")] public int Count { get; set; }
        [JsonProperty("value")] public List<Project> List { get; set; }
    }
}