using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers.Optional;

public class OptionalRepositoryIdentifier
{
    [Display("Repository ID"), DataSource(typeof(RepositoryDataHandler))]
    public string? RepositoryUuid { get; set; } = string.Empty;
}