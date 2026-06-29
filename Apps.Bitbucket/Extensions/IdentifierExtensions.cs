using Apps.Bitbucket.Api;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Models.Entities.Branch;
using Apps.Bitbucket.Models.Identifiers.Optional;

namespace Apps.Bitbucket.Extensions;

public static class IdentifierExtensions
{
    public static string GetBranchName(this OptionalBranchIdentifier optionalBranchIdentifier)
    {
        return !string.IsNullOrEmpty(optionalBranchIdentifier.BranchName) ? optionalBranchIdentifier.BranchName : "HEAD";
    }

    public static async Task<string> ResolveSourceRefAsync(
        this OptionalBranchIdentifier optionalBranchIdentifier,
        BitbucketClient client,
        string workspaceUuid,
        string repositoryUuid)
    {
        if (string.IsNullOrWhiteSpace(optionalBranchIdentifier.BranchName))
            return "HEAD";

        string branchName = Uri.EscapeDataString(optionalBranchIdentifier.BranchName);
        var request = new BitbucketCloudRequest($"repositories/{workspaceUuid}/{repositoryUuid}/refs/branches/{branchName}");
        var branch = await client.ExecuteWithErrorHandling<BranchEntity>(request);

        return branch.Target.Hash;
    }
}
