using Apps.Bitbucket.Helper;
using RestSharp;

namespace Apps.Bitbucket.Extensions;

public static class RestRequestExtensions
{
    public static T AddParameterIfNotEmpty<T>(this T request, string paramName, string? paramValue)
        where T : RestRequest
    {
        if (string.IsNullOrWhiteSpace(paramValue) || string.IsNullOrWhiteSpace(paramValue))
            return request;

        request.AddParameter(paramName, paramValue);
        return request;
    }
    
    public static T AddBitbucketQuery<T>(this T request, Action<CloudQueryBuilder> configureQuery) 
        where T : RestRequest
    {
        var builder = new CloudQueryBuilder();
        configureQuery(builder);

        if (builder.HasConditions)
            request.AddQueryParameter("q", builder.ToString());

        return request;
    }
}