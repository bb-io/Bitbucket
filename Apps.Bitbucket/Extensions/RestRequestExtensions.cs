using Apps.Bitbucket.Helper;
using RestSharp;

namespace Apps.Bitbucket.Extensions;

public static class RestRequestExtensions
{
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