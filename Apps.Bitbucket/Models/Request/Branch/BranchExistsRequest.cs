using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Request.Branch;

public class BranchExistsRequest
{
    [Display("Branch name")]
    public string Name { get; set; } = string.Empty;
}