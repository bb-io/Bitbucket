using Apps.Bitbucket.Authenticators;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Bitbucket.Api;

public class BitbucketClient(IEnumerable<AuthenticationCredentialsProvider> creds) : BlackBirdRestClient(new()
{
    BaseUrl = new Uri("https://api.bitbucket.org/2.0/"),
    Authenticator = new ApiTokenAuthenticator(creds),
})
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject(response.Content);
        var errorMessage = "";

        throw new PluginApplicationException(errorMessage);
    }
}