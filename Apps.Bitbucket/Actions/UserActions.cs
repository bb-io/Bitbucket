using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Entities.User;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Response.User;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Actions;

[ActionList("Users")]
public class UserActions : BitbucketInvocable
{
    public UserActions(InvocationContext invocationContext) : base(invocationContext)
    {
        if (Creds.HasConnectionType(ConnectionTypes.RepoAccessToken))
            throw new PluginMisconfigurationException("The 'Users' actions are not supported for this connection type");
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-users/#api-user-get
    [Action("Get my user", Description = "Get information about my user")]
    public async Task<UserResponse> GetMyUser()
    {
        var request = new BitbucketCloudRequest("user");
        var userEntity = await Client.ExecuteWithErrorHandling<UserEntity>(request);
        return new(userEntity);
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-users/#api-users-selected-user-get
    [Action("Get user", Description = "Get information for a specific user")]
    public async Task<UserResponse> GetUser(
        [ActionParameter] UserIdentifier userIdentifier,
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier   // For the user data handler to work
        )
    {
        var request = new BitbucketCloudRequest($"users/{userIdentifier.UserUuid}");
        var userEntity = await Client.ExecuteWithErrorHandling<UserEntity>(request);
        return new(userEntity);
    }
}