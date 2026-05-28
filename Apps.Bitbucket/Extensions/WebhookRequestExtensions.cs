using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Bitbucket.Extensions;

public static class WebhookRequestExtensions
{
    public static T GetPayload<T>(this WebhookRequest request)
    {
        if (request.Body is null)
            throw new InvalidCastException($"The incoming webhook body for {nameof(T)} was null");
        
        var body = request.Body.ToString();
        if (string.IsNullOrWhiteSpace(body))
            throw new InvalidCastException($"The {nameof(T)} payload was empty");
        
        var response = JsonConvert.DeserializeObject<T>(body) ?? 
                       throw new InvalidCastException($"{nameof(T)} payload could not be deserialized. Raw: {body}");
                       
        return response;
    }
}