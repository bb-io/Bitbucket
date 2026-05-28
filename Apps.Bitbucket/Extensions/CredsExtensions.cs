using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Bitbucket.Extensions;

public static class CredsExtensions
{
    public static bool HasConnectionType(this IEnumerable<AuthenticationCredentialsProvider> creds, string connectionType)
    {
        return creds.Get(CredsNames.ConnectionType).Value.Equals(connectionType, StringComparison.OrdinalIgnoreCase);
    }
}