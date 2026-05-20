using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.Branch;

public class BranchEntity
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("default_merge_strategy")]
    public string DefaultMergeStrategy { get; set; } = string.Empty;

    [JsonProperty("merge_strategies")]
    public IEnumerable<string> MergeStrategies { get; set; } = [];

    [JsonProperty("target")]
    public BranchTarget Target { get; set; } = null!;
}