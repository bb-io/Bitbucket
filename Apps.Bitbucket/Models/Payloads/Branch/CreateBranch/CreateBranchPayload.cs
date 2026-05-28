using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.Branch.CreateBranch;

public class CreateBranchPayload(string branchName, string hash)
{
    [JsonProperty("name")]
    public string Name { get; set; } = branchName;

    [JsonProperty("target")]
    public Target Target { get; set; } = new() { Hash = hash };
}