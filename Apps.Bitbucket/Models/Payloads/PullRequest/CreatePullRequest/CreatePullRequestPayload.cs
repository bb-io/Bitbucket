using Apps.Bitbucket.Models.Request.PullRequest;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Payloads.PullRequest.CreatePullRequest;

public class CreatePullRequestPayload
{
    public CreatePullRequestPayload(CreatePullRequestRequest createInput)
    {
        Title = createInput.Title;
        Source = new Branch { BranchName = new BranchName { Name = createInput.SourceBranchName } };
        IsDraft = createInput.IsDraft ?? false;
        CloseBranchUponMerging = createInput.CloseBranchUponMerging ?? false;
        Description = createInput.Description;
        
        if (!string.IsNullOrWhiteSpace(createInput.TargetBranchName))
            Destination = new Branch { BranchName = new BranchName { Name = createInput.TargetBranchName } };

        if (createInput.ReviewerUuids != null && createInput.ReviewerUuids.Any())
            Reviewers = createInput.ReviewerUuids.Select(x => new Reviewer { Uuid = x }).ToList();
    }
    
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }

    [JsonProperty("source")]
    public Branch Source { get; set; }

    [JsonProperty("destination", NullValueHandling = NullValueHandling.Ignore)]
    public Branch? Destination { get; set; }

    [JsonProperty("reviewers", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Reviewer>? Reviewers { get; set; }

    [JsonProperty("draft", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsDraft { get; set; }

    [JsonProperty("close_source_branch", NullValueHandling = NullValueHandling.Ignore)]
    public bool? CloseBranchUponMerging { get; set; }
}