using RestSharp;

namespace Apps.Bitbucket.Api.Request;

public class BitbucketCloudRequest(string endpoint, Method method = Method.Get, string apiVersion = "2.0")
    : RestRequest($"{apiVersion}/{endpoint.TrimStart('/')}", method);