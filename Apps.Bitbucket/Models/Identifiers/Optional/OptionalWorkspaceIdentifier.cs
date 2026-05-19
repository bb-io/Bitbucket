using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers.Optional;

public class OptionalWorkspaceIdentifier
{
    [Display("Workspace UUID"), DataSource(typeof(WorkspaceDataHandler))]
    public string? WorkspaceUuid { get; set; }
}