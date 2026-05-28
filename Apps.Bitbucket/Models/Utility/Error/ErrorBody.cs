using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Bitbucket.Models.Utility.Error;

public class ErrorBody
{
    [JsonProperty("message")] 
    public string Message { get; set; } = string.Empty;

    [JsonProperty("detail")]
    public JToken? Detail { get; set; }
}