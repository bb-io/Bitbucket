using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Identifiers;

public class FilePathIdentifier
{
    [Display("File path")]
    public string FilePath { get; set; } = string.Empty;
}