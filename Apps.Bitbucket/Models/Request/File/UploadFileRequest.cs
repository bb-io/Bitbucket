using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Bitbucket.Models.Request.File;

public class UploadFileRequest
{
    [Display("File")]
    public FileReference File { get; set; } = null!;

    [Display("File name")]
    public string? FileName { get; set; }

    [Display("Upload commit message")]
    public string? Message { get; set; }
}