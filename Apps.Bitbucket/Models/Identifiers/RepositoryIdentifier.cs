using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Identifiers;

public class RepositoryIdentifier
{
    [Display("Repository UUID")]
    public string RepositoryUuid { get; set; } = string.Empty;
}