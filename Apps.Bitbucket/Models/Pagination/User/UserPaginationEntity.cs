using Apps.Bitbucket.Models.Entities.User;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Models.Pagination.User;

public class UserPaginationEntity
{
    [JsonProperty("user")] 
    public UserEntity UserEntity { get; set; } = null!;
}