using Apps.Bitbucket.Models.Entities.File;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.Diffstat;

public class DiffstatEntity
{
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonProperty("old")]
    public FileEntity OldFile { get; set; } = null!;

    [JsonProperty("new")]
    public FileEntity NewFile { get; set; } = null!;
}