using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp.Authenticators;

namespace Apps.Bitbucket.Authenticators;

public static class AuthenticatorFactory
{
    public static IAuthenticator Create(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        return new ApiTokenAuthenticator(creds);
    }
}