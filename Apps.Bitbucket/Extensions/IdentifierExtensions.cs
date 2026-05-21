using Apps.Bitbucket.Models.Identifiers.Optional;

namespace Apps.Bitbucket.Extensions;

public static class IdentifierExtensions
{
    public static string GetBranchName(this OptionalBranchIdentifier optionalBranchIdentifier)
    {
        return !string.IsNullOrEmpty(optionalBranchIdentifier.BranchName) ? optionalBranchIdentifier.BranchName : "HEAD";
    }
}