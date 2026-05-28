using Apps.Bitbucket.Models.Entities.User;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class PullRequestEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("author")]
    public UserEntity Author { get; set; } = null!;

    [JsonProperty("merge_commit")]
    public MergeCommit MergeCommit { get; set; } = null!;

    [JsonProperty("comment_count")]
    public int CommentCount { get; set; }

    [JsonProperty("task_count")]
    public int TaskCount { get; set; }

    [JsonProperty("close_source_branch")]
    public bool CloseBranchUponMerging { get; set; }

    [JsonProperty("closed_by")]
    public UserEntity? ClosedBy { get; set; }

    [JsonProperty("created_on")]
    public DateTime CreatedOn { get; set; }

    [JsonProperty("updated_on")]
    public DateTime? UpdatedOn { get; set; }

    [JsonProperty("participants")]
    public IEnumerable<Participant> Participants { get; set; } = [];

    [JsonProperty("draft")]
    public bool IsDraft { get; set; }

    [JsonProperty("queued")]
    public bool IsQueued { get; set; }
}