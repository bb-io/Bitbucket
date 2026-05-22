using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Utility.Auth;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.Bitbucket.Authenticators;

public class OAuthAuthenticator(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    private string? _cachedToken;
    
    public async ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            request.AddBearerHeader(_cachedToken);
            return;
        }
        
        string clientId = creds.Get(CredsNames.ClientId).Value;
        string clientSecret = creds.Get(CredsNames.ClientSecret).Value;

        var authClient = new RestClient("https://bitbucket.org/");
        var authRequest = new RestRequest("site/oauth2/access_token", Method.Post)
            .AddBasicAuthHeader(clientId, clientSecret)
            .AddParameter("grant_type", "client_credentials");

        var authResponse = await authClient.ExecuteAsync(authRequest);
        if (string.IsNullOrWhiteSpace(authResponse.Content))
            throw new Exception("No content returned by Bitbucket API in OAuthAuthenticator");
        
        if (!authResponse.IsSuccessful)
        {
            var errorAuthResponse = JsonConvert.DeserializeObject<AuthError>(authResponse.Content);
            string errorMessage = errorAuthResponse?.ErrorDescription ??
                                  authResponse.ErrorMessage ?? 
                                  "Invalid credentials. Please check your inputs";
            throw new PluginApplicationException(errorMessage);
        }
        
        var parsedAuthResponse = JsonConvert.DeserializeObject<AuthResponse>(authResponse.Content) ??
                                 throw new Exception("Could not deserialize auth response in OAuthAuthenticator");
        request.AddBearerHeader(parsedAuthResponse.AccessToken);
        _cachedToken = parsedAuthResponse.AccessToken;
    }
}