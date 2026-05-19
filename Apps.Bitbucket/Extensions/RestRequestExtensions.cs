using RestSharp;

namespace Apps.Bitbucket.Extensions;

public static class RestRequestExtensions
{
    public static RestRequest AddQueryParameterIfNotNull(this RestRequest request, string paramName, string? paramValue)
    {
        if (string.IsNullOrWhiteSpace(paramName) || string.IsNullOrWhiteSpace(paramValue))
            return request;

        return request.AddQueryParameter(paramName, paramValue);
    }
}