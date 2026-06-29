using Apps.Bitbucket.Models.Entities.PullRequest;
using Apps.Bitbucket.Models.Response.File;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.PullRequest;

public record PullRequestResponse
{
    public PullRequestResponse(
        PullRequestEntity pullRequestEntity,
        IEnumerable<FileWebhookResponse>? affectedFiles = null)
    {
        Id = pullRequestEntity.Id;
        Title = pullRequestEntity.Title;
        Description = pullRequestEntity.Description;
        State = pullRequestEntity.State;
        AuthorUuid = pullRequestEntity.Author?.Uuid ?? string.Empty;
        CommentCount = pullRequestEntity.CommentCount;
        TaskCount = pullRequestEntity.TaskCount;
        CloseBranchUponMerging = pullRequestEntity.CloseBranchUponMerging;
        ClosedByUuid = pullRequestEntity.ClosedBy?.Uuid;
        CreatedOn = pullRequestEntity.CreatedOn;
        UpdatedOn = pullRequestEntity.UpdatedOn;
        ParticipantUserUuids = pullRequestEntity.Participants
            .Select(x => x.User?.Uuid)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!);
        SourceBranchName = pullRequestEntity.Source?.Branch?.Name;
        TargetBranchName = pullRequestEntity.Destination?.Branch?.Name;
        SourceCommitHash = pullRequestEntity.Source?.Commit?.Hash;
        TargetCommitHash = pullRequestEntity.Destination?.Commit?.Hash;
        Url = pullRequestEntity.Links?.Html?.Href;
        AffectedFiles = affectedFiles ?? [];
        IsDraft = pullRequestEntity.IsDraft;
        IsQueued = pullRequestEntity.IsQueued;
    }

    [Display("Pull request ID")]
    public string Id { get; set; }

    [Display("Pull request title")]
    public string Title { get; set; }

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Pull request state")]
    public string State { get; set; }

    [Display("Author user ID")]
    public string AuthorUuid { get; set; }

    [Display("Comment count")] 
    public int CommentCount { get; set; }

    [Display("Task count")]
    public int TaskCount { get; set; }

    [Display("Close branch upon merging")]
    public bool CloseBranchUponMerging { get; set; }

    [Display("Closed by user ID")] 
    public string? ClosedByUuid { get; set; }

    [Display("Created on")]
    public DateTime CreatedOn { get; set; }

    [Display("Updated on")]
    public DateTime? UpdatedOn { get; set; }

    [Display("Participant user UUIDs")]
    public IEnumerable<string> ParticipantUserUuids { get; set; }

    [Display("Source branch name")]
    public string? SourceBranchName { get; set; }

    [Display("Target branch name")]
    public string? TargetBranchName { get; set; }

    [Display("Source commit hash")]
    public string? SourceCommitHash { get; set; }

    [Display("Target commit hash")]
    public string? TargetCommitHash { get; set; }

    [Display("URL")]
    public string? Url { get; set; }

    [Display("Affected files")]
    public IEnumerable<FileWebhookResponse> AffectedFiles { get; set; }
    
    [Display("Is draft")]
    public bool IsDraft { get; set; }

    [Display("Is queued")]
    public bool IsQueued { get; set; }
}
