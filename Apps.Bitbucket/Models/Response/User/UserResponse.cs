using Apps.Bitbucket.Models.Entities.User;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.User;

public class UserResponse(UserEntity userEntity)
{
    [Display("User UUID")] 
    public string Uuid { get; set; } = userEntity.Uuid;

    [Display("Created on")] 
    public DateTime CreatedOn { get; set; } = userEntity.CreatedOn;

    [Display("User display name")] 
    public string DisplayName { get; set; } = userEntity.DisplayName;
}