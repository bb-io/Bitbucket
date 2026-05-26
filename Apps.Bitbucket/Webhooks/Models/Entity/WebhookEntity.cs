using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Entity;

public class WebhookEntity
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = string.Empty;
    
    [JsonProperty("url")] 
    public string Url { get; set; } = string.Empty;
}