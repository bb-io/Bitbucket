using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.Bitbucket.Authenticators;

public class OAuthAuthenticator(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        string accessToken = creds.Get(CredsNames.AccessToken).Value;
        request.AddBearerHeader(accessToken);
        return ValueTask.CompletedTask;
    }
}