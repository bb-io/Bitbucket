using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Entity.Push;

public class PushBody
{
    [JsonProperty("changes")] 
    public List<Change> Changes { get; set; } = [];
}