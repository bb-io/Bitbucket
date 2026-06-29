using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Identifiers.Optional;

public class OptionalPullRequestFilter
{
    [Display("Pull request title contains")]
    public IEnumerable<string>? TitleContains { get; set; }

    [Display("Pull request title doesn't contain")]
    public IEnumerable<string>? TitleDoesntContain { get; set; }

    [Display("Pull request description contains")]
    public IEnumerable<string>? DescriptionContains { get; set; }

    [Display("Pull request description doesn't contain")]
    public IEnumerable<string>? DescriptionDoesntContain { get; set; }
}
