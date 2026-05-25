using System.Globalization;
using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Utility.Auth;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Bitbucket.Connections.OAuth;

public class OAuth2TokenService(InvocationContext context) : BaseInvocable(context), IOAuth2TokenService, ITokenRefreshable
{
    private const string TokenUrl = "https://bitbucket.org/site/oauth2/access_token";
    
    public async Task<Dictionary<string, string>> RequestToken(
        string state, 
        string code, 
        Dictionary<string, string> values, 
        CancellationToken cancellationToken)
    {
        string clientId = values.TryGetValue(CredsNames.ClientId, out var id) ? id : string.Empty;
        string clientSecret = values.TryGetValue(CredsNames.ClientSecret, out var secret) ? secret : string.Empty;

        var client = new RestClient();
        var request = new RestRequest(TokenUrl, Method.Post);
        
        request.AddParameter("grant_type", "authorization_code");
        request.AddParameter("code", code);
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode");

        var response = await client.ExecuteAsync(request, cancellationToken);
        if (!response.IsSuccessful)
        {
            InvocationContext.Logger?.LogError(
                $"[BitbucketCloudOAuth2] OAuth request token response is {response.StatusCode}. Response: {response.Content}",
                []);
            throw new Exception($"Failed to get token: {response.Content}");
        }

        var tokenData = JsonConvert.DeserializeObject<AuthResponse>(response.Content ?? string.Empty);
        if (tokenData is not null)
        {
            return new Dictionary<string, string>
            {
                { CredsNames.AccessToken, $"Bearer {tokenData.AccessToken}" },
                { CredsNames.RefreshToken, tokenData.RefreshToken },
                { CredsNames.ExpiresIn, DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn).ToString(CultureInfo.InvariantCulture) }
            };
        }
        
        InvocationContext.Logger?.LogError(
            $"[BitbucketCloudOAuth2] Could not deserialize OAuth response. Response: {response.Content}",
            []);
        throw new Exception("Could not deserialize OAuth response");

    }

    public async Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values, CancellationToken ct)
    {
        string clientId = values.TryGetValue(CredsNames.ClientId, out var id) ? id : string.Empty;
        string clientSecret = values.TryGetValue(CredsNames.ClientSecret, out var secret) ? secret : string.Empty;
        string refreshToken = values.TryGetValue(CredsNames.RefreshToken, out var refresh) ? refresh : string.Empty;

        if (string.IsNullOrEmpty(refreshToken))
        {
            InvocationContext.Logger?.LogError(
                $"[BitbucketCloudOAuth2] No refresh token found. Values: {values.ToLogString()}", 
                []);
            throw new Exception("No refresh token found");
        }

        var client = new RestClient();
        var request = new RestRequest(TokenUrl, Method.Post);
        
        request.AddParameter("grant_type", "refresh_token");
        request.AddParameter("refresh_token", refreshToken);
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_secret", clientSecret);
        request.AddParameter("redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode");

        var response = await client.ExecuteAsync(request, ct);
        if (!response.IsSuccessful)
        {
            InvocationContext.Logger?.LogError($"[BitbucketCloudOAuth2] Refresh token failed: {response.Content}", []);
            throw new Exception($"Failed to refresh token: {response.Content}");
        }

        var tokenData = JsonConvert.DeserializeObject<AuthResponse>(response.Content ?? string.Empty);
        if (tokenData is not null)
        {
            return new Dictionary<string, string>
            {
                { CredsNames.AccessToken, $"Bearer {tokenData.AccessToken}" },
                { CredsNames.RefreshToken, string.IsNullOrEmpty(tokenData.RefreshToken) ? refreshToken : tokenData.RefreshToken },
                { CredsNames.ExpiresIn, DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn).ToString(CultureInfo.InvariantCulture) }
            };
        }
        
        InvocationContext.Logger?.LogError(
            $"[BitbucketCloudOAuth2] Could not deserialize OAuth refresh response: {response.Content}", 
            []);
        throw new Exception("Could not deserialize OAuth refresh response");
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
    {
        return 
            values.TryGetValue(CredsNames.ExpiresIn, out var expiresIn) && 
            DateTime.UtcNow > DateTime.Parse(expiresIn, CultureInfo.InvariantCulture);
    }

    public int? GetRefreshTokenExprireInMinutes(Dictionary<string, string> values)
    {
        if (!values.TryGetValue(CredsNames.ExpiresIn, out var expireValue))
            return null;

        if (!DateTime.TryParse(expireValue, CultureInfo.InvariantCulture, out var expireDate))
            return null;

        var difference = expireDate - DateTime.UtcNow;
        if (difference.TotalMinutes <= 0)
            return 0;

        return (int)difference.TotalMinutes - 5;
    }
}