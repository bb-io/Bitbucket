using Apps.Bitbucket.Models.Entities.Branch;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Entity.Push;

public class ChangeState
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("target")]
    public BranchTarget Target { get; set; } = null!;
}