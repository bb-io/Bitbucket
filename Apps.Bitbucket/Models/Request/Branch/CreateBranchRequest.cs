using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Request.Branch;

public class CreateBranchRequest
{
    [Display("Branch name")]
    public string BranchName { get; set; } = string.Empty;

    [Display("Source branch name"), DataSource(typeof(BranchDataHandler))]
    public string SourceBranchName { get; set; } = string.Empty;
}