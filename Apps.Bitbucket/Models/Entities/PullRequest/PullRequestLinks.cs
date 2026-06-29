using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class PullRequestLinks
{
    [JsonProperty("html")]
    public PullRequestLink? Html { get; set; }

    [JsonProperty("diffstat")]
    public PullRequestLink? Diffstat { get; set; }
}
