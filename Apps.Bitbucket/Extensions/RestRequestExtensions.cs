using System.Text;
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

    public static T AddBasicAuthHeader<T>(this T request, string username, string password)
        where T : RestRequest
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return request;
        
        byte[] credentialBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        string encodedCredentials = Convert.ToBase64String(credentialBytes);
        
        request.AddOrUpdateHeader("Authorization", $"Basic {encodedCredentials}");
        return request;
    }

    public static T AddBearerHeader<T>(this T request, string bearer)
        where T : RestRequest
    {
        if (string.IsNullOrEmpty(bearer))
            return request;
        
        request.AddOrUpdateHeader("Authorization", $"Bearer {bearer}");
        return request;
    }
}