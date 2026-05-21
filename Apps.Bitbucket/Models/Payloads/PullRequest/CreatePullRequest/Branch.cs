using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.PullRequest.CreatePullRequest;

public class Branch
{
    [JsonProperty("branch")]
    public BranchName BranchName { get; set; } = null!;
}