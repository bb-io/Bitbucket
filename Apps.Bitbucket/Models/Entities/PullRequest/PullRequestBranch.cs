using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class PullRequestBranch
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
