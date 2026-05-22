using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Utility.Auth;

public class AuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
}