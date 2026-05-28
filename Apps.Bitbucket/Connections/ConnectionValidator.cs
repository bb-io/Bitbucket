using System.Net;
using Apps.Bitbucket.Api;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Bitbucket.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> creds,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new BitbucketClient(creds);
            BitbucketCloudRequest request = CreateValidationRequest(InvocationContext.AuthenticationCredentialsProviders);
            var response = await client.ExecuteAsync(request, cancellationToken);

            var isValid =
                response.StatusCode != HttpStatusCode.Unauthorized &&
                response.StatusCode != HttpStatusCode.Forbidden &&
                response.StatusCode != HttpStatusCode.NotFound;
            
            return new ConnectionValidationResponse
            {
                IsValid = isValid,
                Message = isValid ? "Success" : response.Content ?? response.ErrorMessage ?? response.StatusCode.ToString(),
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

    private static BitbucketCloudRequest CreateValidationRequest(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var credsList = creds.ToList();
        BitbucketCloudRequest request;
        
        string connectionType = credsList.Get(CredsNames.ConnectionType).Value;
        switch (connectionType)
        {
            case ConnectionTypes.RepoAccessToken:
                string repositoryUuid = credsList.Get(CredsNames.RepositoryUuid).Value;
                string workspaceUuid = credsList.Get(CredsNames.WorkspaceUuid).Value;
                request = new BitbucketCloudRequest($"repositories/{workspaceUuid}/{repositoryUuid}");
                break;
            default:
                request = new BitbucketCloudRequest("user");
                break;
        }

        return request;
    }
}