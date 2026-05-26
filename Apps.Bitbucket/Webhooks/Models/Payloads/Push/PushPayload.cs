using Apps.Bitbucket.Models.Entities.Repository;
using Apps.Bitbucket.Webhooks.Models.Entity.Push;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Payloads.Push;

public class PushPayload
{
    [JsonProperty("actor")] 
    public Actor Actor { get; set; } = null!;

    [JsonProperty("repository")] 
    public RepositoryEntity Repository { get; set; } = null!;

    [JsonProperty("push")] 
    public PushBody Push { get; set; } = null!;
}