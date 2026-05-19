using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.User;

public class UserEntity
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonProperty("created_on")]
    public DateTime CreatedOn { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}