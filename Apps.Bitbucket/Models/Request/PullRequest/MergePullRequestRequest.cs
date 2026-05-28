using Apps.Bitbucket.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Bitbucket.Models.Request.PullRequest;

public class MergePullRequestRequest
{
    [Display("Commit message")]
    public string? CommitMessage { get; set; }
    
    [Display("Close branch upon merging", Description = "If empty, the default pull request setting will be applied")]
    public bool? CloseBranchUponMerging { get; set; }

    [Display("Merge strategy"), StaticDataSource(typeof(MergeStrategyDataHandler))] 
    public string? MergeStrategy { get; set; }
}