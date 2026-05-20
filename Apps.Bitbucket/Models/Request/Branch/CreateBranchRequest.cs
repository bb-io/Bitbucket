using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Request.Branch;

public class CreateBranchRequest
{
    [Display("Branch name")]
    public string BranchName { get; set; } = string.Empty;

    [Display("Source branch name")]
    public string SourceBranchName { get; set; } = string.Empty;
}