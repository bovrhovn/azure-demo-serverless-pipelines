using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Demo.Cli.Models
{
    public class BuildModel
    {
        [JsonProperty("count")] public int Count { get; set; }
        [JsonProperty("value")] public List<Build> List { get; set; }
    }

    public class Build
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("reason")] public string Reason { get; set; }
        [JsonProperty("result")] public string Result { get; set; }
        [JsonProperty("queueTime")] public DateTime QueueTime { get; set; }
        [JsonProperty("startTime")] public DateTime StartTime { get; set; }
        [JsonProperty("finishTime")] public DateTime FinishTime { get; set; }
        [JsonProperty("requestedBy")] public RequestedBy RequestedBy { get; set; }
        [JsonProperty("queue")] public Queue Queue { get; set; }
    }

    public class Pool
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("isHosted")] public bool IsHosted { get; set; }
    }

    public class Queue
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("pool")] public Pool Pool { get; set; }
    }


    public class Avatar
    {
        [JsonProperty("href")] public string Href { get; set; }
    }

    public class Links
    {
        [JsonProperty("avatar")] public Avatar Avatar { get; set; }
    }

    public class RequestedBy
    {
        [JsonProperty("displayName")] public string DisplayName { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("_links")] public Links Links { get; set; }

        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("uniqueName")] public string UniqueName { get; set; }

        [JsonProperty("imageUrl")] public string ImageUrl { get; set; }

        [JsonProperty("descriptor")] public string Descriptor { get; set; }
    }
}