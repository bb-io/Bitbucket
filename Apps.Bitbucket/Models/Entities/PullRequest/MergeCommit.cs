using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class MergeCommit
{
    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;
}