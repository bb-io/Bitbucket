using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Request.PullRequest;

public class CreatePullRequestRequest
{
    [Display("Pull request title")]
    public string Title { get; set; } = string.Empty;

    [Display("Source branch name"), DataSource(typeof(BranchDataHandler))]
    public string SourceBranchName { get; set; } = string.Empty;

    [Display("Target branch name", Description = "If not specified, it will default to the source branch name")] 
    [DataSource(typeof(BranchDataHandler))]
    public string? TargetBranchName { get; set; }

    [Display("Reviewer UUIDs"), DataSource(typeof(UserDataHandler))]
    public IEnumerable<string>? ReviewerUuids { get; set; }

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Is draft", Description = "False by default")]
    public bool? IsDraft { get; set; }

    [Display("Close branch upon merging", Description = "False by default")]
    public bool? CloseBranchUponMerging { get; set; }
}