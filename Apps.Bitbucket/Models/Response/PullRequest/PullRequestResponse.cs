using Apps.Bitbucket.Models.Entities.PullRequest;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.PullRequest;

public record PullRequestResponse
{
    public PullRequestResponse(PullRequestEntity pullRequestEntity)
    {
        Id = pullRequestEntity.Id;
        Title = pullRequestEntity.Title;
        State = pullRequestEntity.State;
        AuthorUuid = pullRequestEntity.Author.Uuid;
        CommentCount = pullRequestEntity.CommentCount;
        TaskCount = pullRequestEntity.TaskCount;
        CloseBranchUponMerging = pullRequestEntity.CloseBranchUponMerging;
        ClosedByUuid = pullRequestEntity.ClosedBy?.Uuid;
        CreatedOn = pullRequestEntity.CreatedOn;
        UpdatedOn = pullRequestEntity.UpdatedOn;
        ParticipantUserUuids = pullRequestEntity.Participants.Select(x => x.User.Uuid);
        IsDraft = pullRequestEntity.IsDraft;
        IsQueued = pullRequestEntity.IsQueued;
    }

    [Display("Pull request ID")]
    public string Id { get; set; }

    [Display("Pull request title")]
    public string Title { get; set; }

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
    
    [Display("Is draft")]
    public bool IsDraft { get; set; }

    [Display("Is queued")]
    public bool IsQueued { get; set; }
}