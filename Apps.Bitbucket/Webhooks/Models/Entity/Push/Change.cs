using Newtonsoft.Json;

namespace Apps.Bitbucket.Webhooks.Models.Entity.Push;

public class Change
{
    [JsonProperty("new")]
    public ChangeState? New { get; set; }

    [JsonProperty("old")]
    public ChangeState? Old { get; set; }

    [JsonProperty("commits")]
    public List<PushCommit> Commits { get; set; } = [];
}
