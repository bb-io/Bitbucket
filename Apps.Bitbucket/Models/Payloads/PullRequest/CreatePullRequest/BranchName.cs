using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.PullRequest.CreatePullRequest;

public class BranchName
{
    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;
}