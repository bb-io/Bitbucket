using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.Branch.CreateBranch;

public class Target
{
    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;
}