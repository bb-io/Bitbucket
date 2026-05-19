using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Identifiers;

public class UserIdentifier
{
    [Display("User UUID")] 
    public string UserUuid { get; set; } = string.Empty;
}