using Apps.Bitbucket.Models.Entities.Branch;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.Branch;

public record BranchResponse
{
    public BranchResponse(BranchEntity branchEntity)
    {
        Name = branchEntity.Name;
        DefaultMergeStrategy = branchEntity.DefaultMergeStrategy;
        MergeStrategies = branchEntity.MergeStrategies;
    }
    
    [Display("Branch name")]
    public string Name { get; set; }

    [Display("Default merge strategy")]
    public string DefaultMergeStrategy { get; set; }

    [Display("Merge strategies")]
    public IEnumerable<string> MergeStrategies { get; set; } = [];
};