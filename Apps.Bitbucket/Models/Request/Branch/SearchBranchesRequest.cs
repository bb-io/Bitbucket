using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Request.Branch;

public class SearchBranchesRequest
{
    [Display("Branch name contains")]
    public string? BranchNameContains { get; set; }
}