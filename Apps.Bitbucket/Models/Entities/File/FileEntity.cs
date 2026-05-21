using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.File;

public class FileEntity
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonProperty("path")]
    public string Path { get; set; } = string.Empty;
}