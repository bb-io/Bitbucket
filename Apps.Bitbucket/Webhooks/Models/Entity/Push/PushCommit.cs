using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Entity.Push;

public class PushCommit
{
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}
