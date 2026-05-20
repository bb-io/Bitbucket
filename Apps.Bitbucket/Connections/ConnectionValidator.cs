using Apps.Bitbucket.Api;
using Apps.Bitbucket.Api.Request;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new BitbucketClient(authenticationCredentialsProviders);
            var request = new BitbucketCloudRequest("user");
            var response = await client.ExecuteAsync(request, cancellationToken);

            var isValid = response.StatusCode != System.Net.HttpStatusCode.Unauthorized;
            return new ConnectionValidationResponse
            {
                IsValid = isValid,
                Message = isValid ? "Success" : (response.Content ?? response.ErrorMessage ?? response.StatusCode.ToString()),
            };

        } 
        catch(Exception ex)
        {
            InvocationContext.Logger?.LogError($"Connection validation failed: {ex.Message}", []);

            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }

    }
}