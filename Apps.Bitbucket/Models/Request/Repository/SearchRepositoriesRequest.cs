using Apps.Bitbucket.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Bitbucket.Models.Request.Repository;

public class SearchRepositoriesRequest
{
    [Display("Repository name contains")]
    public string? NameContains { get; set; }

    [Display("Repository language"), StaticDataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }
}