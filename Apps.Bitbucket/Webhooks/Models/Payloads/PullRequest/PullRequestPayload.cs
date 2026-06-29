using Apps.Bitbucket.Models.Entities.PullRequest;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Payloads.PullRequest;

public class PullRequestPayload
{
    [JsonProperty("pullrequest")]
    public PullRequestEntity? PullRequest { get; set; }
}
