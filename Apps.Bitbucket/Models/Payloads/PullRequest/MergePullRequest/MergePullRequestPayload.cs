using Apps.Bitbucket.Models.Request.PullRequest;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.PullRequest.MergePullRequest;

public class MergePullRequestPayload(MergePullRequestRequest request)
{
    [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
    public string? CommitMessage { get; set; } = request.CommitMessage;

    [JsonProperty("close_source_branch", NullValueHandling = NullValueHandling.Ignore)]
    public bool? CloseBranchUponMerging { get; set; } = request.CloseBranchUponMerging;

    [JsonProperty("merge_strategy", NullValueHandling = NullValueHandling.Ignore)]
    public string? MergeStrategy { get; set; } = request.MergeStrategy;
}