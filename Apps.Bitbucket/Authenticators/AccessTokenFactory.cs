using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.Bitbucket.Authenticators;

public class AccessTokenFactory(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        string apiToken = creds.Get(CredsNames.AccessToken).Value;
        
        request.AddHeader("Authorization", $"Bearer {apiToken}");
        return ValueTask.CompletedTask;
    }
}