using System.Net;
using Apps.Bitbucket.Api.Request;
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
    BaseUrl = new Uri("https://api.bitbucket.org/"),
    Authenticator = AuthenticatorFactory.Create(creds),
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

    public async Task<IEnumerable<T>> PaginateOnce<T>(BitbucketCloudRequest request)
    {
        var result = await ExecuteWithErrorHandling<PaginationResponse<T>>(request);
        return result.Values;
    }

    public async Task<IEnumerable<T>> Paginate<T>(BitbucketCloudRequest request)
    {
        List<T> resultValues = [];
        PaginationResponse<T> paginationResponse = await ExecuteWithErrorHandling<PaginationResponse<T>>(request);
        resultValues.AddRange(paginationResponse.Values);
        
        while (!string.IsNullOrWhiteSpace(paginationResponse.Next))
        {
            var nextRequest = new RestRequest(paginationResponse.Next);
        
            paginationResponse = await ExecuteWithErrorHandling<PaginationResponse<T>>(nextRequest);
            resultValues.AddRange(paginationResponse.Values);
        }

        return resultValues;
    }
    
    public async Task<RestResponse> ExecuteWithExpectedStatuses(
        RestRequest request, 
        params HttpStatusCode[] expectedStatusCodes)
    {
        var response = await ExecuteAsync(request);

        if (response.IsSuccessStatusCode || expectedStatusCodes.Contains(response.StatusCode))
            return response;

        throw ConfigureErrorException(response);
    }
}