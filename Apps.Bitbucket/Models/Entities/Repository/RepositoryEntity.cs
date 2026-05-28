using Apps.Bitbucket.Models.Entities.User;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Entities.Repository;

public class RepositoryEntity
{
    [JsonProperty("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonProperty("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("is_private")]
    public bool IsPrivate { get; set; }

    [JsonProperty("owner")]
    public UserEntity Owner { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("created_on")]
    public DateTime CreatedOn { get; set; }

    [JsonProperty("updated_on")]
    public DateTime? UpdatedOn { get; set; }

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("has_issues")]
    public bool HasIssues { get; set; }
    
    [JsonProperty("has_wiki")]
    public bool HasWiki { get; set; }

    [JsonProperty("fork_policy")]
    public string ForkPolicy { get; set; } = string.Empty;
}