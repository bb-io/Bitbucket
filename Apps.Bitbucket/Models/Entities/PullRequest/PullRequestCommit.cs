using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class PullRequestCommit
{
    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;
}
