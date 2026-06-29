using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class PullRequestLink
{
    [JsonProperty("href")]
    public string Href { get; set; } = string.Empty;
}
