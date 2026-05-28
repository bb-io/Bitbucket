using Apps.Bitbucket.Models.Entities.User;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.PullRequest;

public class Participant
{
    [JsonProperty("user")]
    public UserEntity User { get; set; } = null!;
}