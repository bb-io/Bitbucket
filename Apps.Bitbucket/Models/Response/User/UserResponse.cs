using Apps.Bitbucket.Models.Entities.User;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.User;

public record UserResponse
{
    public UserResponse(UserEntity userEntity)
    {
        Uuid = userEntity.Uuid;
        CreatedOn = userEntity.CreatedOn;
        DisplayName = userEntity.DisplayName;
    }

    [Display("User UUID")] 
    public string Uuid { get; set; }

    [Display("Created on")] 
    public DateTime CreatedOn { get; set; }

    [Display("User display name")] 
    public string DisplayName { get; set; }
}