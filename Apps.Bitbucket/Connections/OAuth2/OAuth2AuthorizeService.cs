using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.AspNetCore.WebUtilities;

namespace Apps.Bitbucket.Connections.OAuth2;

public class OAuth2AuthorizeService(InvocationContext context) : BitbucketInvocable(context), IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> values)
    {
        string bridgeOauthUrl = $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/oauth";
        string clientId = values.TryGetValue(CredsNames.ClientId, out var id) ? id : string.Empty;

        var parameters = new Dictionary<string, string>
        {
            { "client_id", clientId }, 
            { "response_type", "code" },
            { "state", values["state"] }, 
            { "authorization_url", "https://bitbucket.org/site/oauth2/authorize" },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "actual_redirect_uri", InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString() }
        };

        return QueryHelpers.AddQueryString(bridgeOauthUrl, parameters!);
    }
}