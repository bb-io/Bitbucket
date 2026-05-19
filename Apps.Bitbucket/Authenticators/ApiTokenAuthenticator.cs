using System.Text;
using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.Bitbucket.Authenticators;

public class ApiTokenAuthenticator(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        string username = creds.Get(CredsNames.Username).Value;
        string apiToken = creds.Get(CredsNames.ApiToken).Value;
        
        byte[] credentialBytes = Encoding.UTF8.GetBytes($"{username}:{apiToken}");
        string encodedCredentials = Convert.ToBase64String(credentialBytes);
        
        request.AddOrUpdateHeader("Authorization", $"Basic {encodedCredentials}");
        return ValueTask.CompletedTask;
    }
}