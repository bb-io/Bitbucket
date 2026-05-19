using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Utility.Error;

public class ErrorResponse
{
    [JsonProperty("error")] 
    public ErrorBody Error { get; set; } = null!;
}