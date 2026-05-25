using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Bitbucket.Helper;

public class IdentifierResolver(IEnumerable<AuthenticationCredentialsProvider> creds)
{
    public string ResolveWorkspaceUuid(string? workspaceInput)
    {
        if (creds.HasConnectionType(ConnectionTypes.RepoAccessToken))
            return creds.Get(CredsNames.WorkspaceUuid).Value;

        return string.IsNullOrWhiteSpace(workspaceInput) 
            ? throw new PluginMisconfigurationException("Please specify a workspace UUID") 
            : workspaceInput;
    }

    public string ResolveRepositoryUuid(string? repositoryInput)
    {
        if (creds.HasConnectionType(ConnectionTypes.RepoAccessToken))
            return creds.Get(CredsNames.RepositoryUuid).Value;

        return string.IsNullOrWhiteSpace(repositoryInput) 
            ? throw new PluginMisconfigurationException("Please specify a repository UUID") 
            : repositoryInput;
    }
}