using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.PullRequest.CreatePullRequest;

public class Reviewer
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = string.Empty;
}