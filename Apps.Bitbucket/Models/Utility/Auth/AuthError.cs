using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Utility.Auth;

public class AuthError
{
    [JsonProperty("error_description")]
    public string ErrorDescription { get; set; } = string.Empty;
}