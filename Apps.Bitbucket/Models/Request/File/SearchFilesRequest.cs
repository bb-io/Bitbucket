using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Request.File;

public class SearchFilesRequest
{
    [Display("Include subfolders", Description = "False by default")]
    public bool? IncludeSubfolders { get; set; }

    [Display("Maximum depth", Description = "The maximum number of folder levels to search. Defaults to 5")]
    public int? MaxDepth { get; set; }

    [Display("Path name contains")]
    public string? PathNameContains { get; set; }
}