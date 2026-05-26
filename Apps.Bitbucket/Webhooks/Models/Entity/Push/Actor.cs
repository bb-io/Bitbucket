using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Entity.Push;

public class Actor
{
    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}