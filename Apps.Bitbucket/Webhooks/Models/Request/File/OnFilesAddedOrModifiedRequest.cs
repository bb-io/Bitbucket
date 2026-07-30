using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Webhooks.Models.Request.File;

public class OnFilesAddedOrModifiedRequest
{
    [Display("File extensions")]
    public List<string>? FileExtensions { get; set; }
}