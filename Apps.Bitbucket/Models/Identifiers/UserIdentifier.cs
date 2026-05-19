using Apps.Bitbucket.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Models.Identifiers;

public class UserIdentifier
{
    [Display("User UUID"), DataSource(typeof(UserDataHandler))]
    public string UserUuid { get; set; } = string.Empty;
}