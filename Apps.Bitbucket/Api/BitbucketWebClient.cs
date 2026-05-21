using Apps.Bitbucket.Authenticators;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Bitbucket.Api;

public class BitbucketWebClient(IEnumerable<AuthenticationCredentialsProvider> creds) : BlackBirdRestClient(new()
{
    BaseUrl = new Uri("https://bitbucket.org/"),
    Authenticator = AuthenticatorFactory.Create(creds),
})
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        string statusCodePart = $"Status code {(int)response.StatusCode} ({response.StatusCode}).";
        return new PluginApplicationException(statusCodePart);
    }
}