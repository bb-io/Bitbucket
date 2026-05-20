using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.Branch;

public class BranchTarget
{
    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;
}