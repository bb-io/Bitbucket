using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Utility.Error;

public class ErrorBody
{
    [JsonProperty("message")] 
    public string Message { get; set; } = string.Empty;

    [JsonProperty("detail")]
    public string? Detail { get; set; }
}