using System.Text.RegularExpressions;
using Apps.Bitbucket.Models.Entities.Diffstat;
using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Response.File;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Bitbucket.Webhooks.Helpers;

public static class WebhookFilterHelper
{
    public static bool BranchMatches(string actualBranchName, OptionalBranchIdentifier? branchIdentifier)
    {
        if (branchIdentifier is null)
            return true;

        if (!string.IsNullOrWhiteSpace(branchIdentifier.BranchName) &&
            !string.Equals(branchIdentifier.BranchName, actualBranchName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var contains = Normalize(branchIdentifier.BranchNameContains);

        if (contains.Count > 0 &&
            !contains.Any(value => actualBranchName.Contains(value, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var doesntContain = Normalize(branchIdentifier.BranchNameDoesntContain);

        return doesntContain.Count == 0 ||
               !doesntContain.Any(value => actualBranchName.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    public static List<FileWebhookResponse> CreateFileResponses(
        IEnumerable<DiffstatEntity> diffstats,
        IEnumerable<string>? fileStatuses,
        IEnumerable<string>? rawPatterns,
        string branchName,
        string? commitMessage = null)
    {
        var statuses = Normalize(fileStatuses);

        return diffstats
            .Where(diffstat => statuses.Count == 0 || statuses.Contains(diffstat.Status))
            .Select(diffstat => diffstat.NewFile ?? diffstat.OldFile)
            .Where(file => file is not null)
            .Select(file => CreateFileResponse(file!, rawPatterns, branchName, commitMessage))
            .Where(file => file is not null)
            .Select(file => file!)
            .ToList();
    }

    public static FileWebhookResponse? CreateFileResponse(
        FileEntity file,
        IEnumerable<string>? rawPatterns,
        string branchName,
        string? commitMessage = null)
    {
        var patterns = Normalize(rawPatterns);

        if (patterns.Count == 0)
            return new(file, branchName, null, commitMessage);

        foreach (var pattern in patterns)
        {
            Regex regex;
            try
            {
                regex = new Regex(pattern);
            }
            catch (ArgumentException exception)
            {
                throw new PluginMisconfigurationException(
                    $"Invalid file path regex pattern '{pattern}': {exception.Message}");
            }

            var groupNames = regex.GetGroupNames()
                .Where(name => name != "0")
                .ToList();

            if (groupNames.Count > 1)
            {
                throw new PluginMisconfigurationException(
                    $"File path regex pattern '{pattern}' must contain no more than one capture group.");
            }

            var match = regex.Match(file.Path);
            if (!match.Success)
                continue;

            var extractedPart = groupNames.Count == 1
                ? match.Groups[groupNames[0]].Value
                : null;

            return new(file, branchName, extractedPart, commitMessage);
        }

        return null;
    }

    private static List<string> Normalize(IEnumerable<string>? values)
    {
        return values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList() ?? [];
    }
}
