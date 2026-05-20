using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Bitbucket.Extensions;

public static class StringExtensions
{
    public static bool MatchesSearch(this string? source, string? searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        
        return source?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true;
    }
}