using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers;

public class RepositoryIdentifier
{
    [Display("Repository UUID"), DataSource(typeof(RepositoryDataHandler))]
    public string RepositoryUuid { get; set; } = string.Empty;
}