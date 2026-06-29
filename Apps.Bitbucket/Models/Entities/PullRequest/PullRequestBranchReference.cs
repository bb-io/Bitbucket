using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class PullRequestBranchReference
{
    [JsonProperty("branch")]
    public PullRequestBranch? Branch { get; set; }

    [JsonProperty("commit")]
    public PullRequestCommit? Commit { get; set; }
}
