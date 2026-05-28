using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Request.File;

public class DeleteFileRequest
{
    [Display("Delete commit message")]
    public string? Message { get; set; }
}