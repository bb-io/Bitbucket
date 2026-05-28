using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp.Authenticators;

namespace Apps.Bitbucket.Authenticators;

public static class AuthenticatorFactory
{
    public static IAuthenticator Create(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var credsList = creds.ToList();
        string connectionType = credsList.Get(CredsNames.ConnectionType).Value;
        
        return connectionType switch
        {
            ConnectionTypes.ApiToken => new ApiTokenAuthenticator(credsList),
            ConnectionTypes.OAuth2 => new OAuthAuthenticator(credsList),
            ConnectionTypes.RepoAccessToken => new AccessTokenFactory(credsList),
            _ => throw new Exception($"Unknown connection type was passed to AuthenticatorFactory: {connectionType}")
        };
    }
}