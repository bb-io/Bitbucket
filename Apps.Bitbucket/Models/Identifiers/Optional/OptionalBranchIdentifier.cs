using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers.Optional;

public class OptionalBranchIdentifier
{
    [Display("Branch name"), DataSource(typeof(BranchDataHandler))]
    public string? BranchName { get; set; }
}