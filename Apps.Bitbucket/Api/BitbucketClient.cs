using Apps.Bitbucket.Authenticators;
using Apps.Bitbucket.Models.Pagination;
using Apps.Bitbucket.Models.Utility.Error;
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
        string statusCodePart = $"Status code {(int)response.StatusCode} ({response.StatusCode}).";
        
        string responseContent = !string.IsNullOrWhiteSpace(response.Content)
            ? response.Content
            : throw new PluginApplicationException($"{statusCodePart} The server did not return any content.");
        
        var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
        var errorObject = error?.Error;
        
        string errorMessage = errorObject?.Message ?? "Unknown error. ";
        if (!string.IsNullOrWhiteSpace(errorObject?.Detail))
            errorMessage = $"{errorObject.Detail} - {errorMessage}";

        throw new PluginApplicationException($"{statusCodePart} {errorMessage}");
    }

    public async Task<IEnumerable<T>> PaginateOnce<T>(RestRequest request)
    {
        var result = await ExecuteWithErrorHandling<PaginationResponse<T>>(request);
        return result.Values;
    }
}