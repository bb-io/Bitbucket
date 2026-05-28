using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Bitbucket.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = ConnectionTypes.ApiToken,
            DisplayName = "API Token",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.Username) { DisplayName = "Account email" },
                new(CredsNames.ApiToken) { DisplayName = "API Token", Sensitive = true }
            }
        },
        new()
        {
            Name = ConnectionTypes.OAuth2,
            DisplayName = "OAuth2",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ClientId) { DisplayName = "Client ID" },
                new(CredsNames.ClientSecret) { DisplayName = "Client secret", Sensitive = true } 
            }
        },
        new()
        {
            Name = ConnectionTypes.RepoAccessToken,
            DisplayName = "Repository access token",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.WorkspaceUuid) { DisplayName = "Workspace UUID or name" },
                new(CredsNames.RepositoryUuid) { DisplayName = "Repository UUID or name" },
                new(CredsNames.AccessToken) { DisplayName = "Access token", Sensitive = true }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        var providers = values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();

        var connectionType = values[nameof(ConnectionPropertyGroup)] switch
        {
            var ct when ConnectionTypes.SupportedConnectionTypes.Contains(ct) => ct,
            _ => throw new Exception($"Unknown connection type: {values[nameof(ConnectionPropertyGroup)]}")
        };

        providers.Add(new AuthenticationCredentialsProvider(CredsNames.ConnectionType, connectionType));
        return providers;
    }
}