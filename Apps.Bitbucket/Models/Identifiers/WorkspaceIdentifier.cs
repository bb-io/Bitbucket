using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers;

public class WorkspaceIdentifier
{
    [Display("Workspace UUID"), DataSource(typeof(WorkspaceDataHandler))]
    public string WorkspaceUuid { get; set; } = string.Empty;
}