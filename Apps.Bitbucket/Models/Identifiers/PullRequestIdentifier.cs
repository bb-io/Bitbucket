using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers;

public class PullRequestIdentifier
{
    [Display("Pull request ID"), DataSource(typeof(PullRequestDataHandler))]
    public string PullRequestId { get; set; } = string.Empty;
}