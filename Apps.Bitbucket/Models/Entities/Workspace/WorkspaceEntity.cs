using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.Workspace;

public class WorkspaceEntity
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonProperty("slug")]
    public string Slug { get; set; } = string.Empty;
}