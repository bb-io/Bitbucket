using Apps.Bitbucket.Models.Entities.Workspace;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Pagination.Workspace;

public class WorkspacePaginationEntity
{
    [JsonProperty("workspace")] 
    public WorkspaceEntity WorkspaceEntity { get; set; } = null!;
}