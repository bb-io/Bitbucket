using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Identifiers.Optional;

public class OptionalFilepath
{
    [Display("File path patterns", Description = "C# regex patterns. Each pattern can contain no capture group or one capture group.")]
    public IEnumerable<string>? FilePathPatterns { get; set; }
}
