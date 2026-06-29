using Apps.Bitbucket.Models.Entities.File;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.File;

public record FileWebhookResponse
{
    public FileWebhookResponse(FileEntity file, string branchName, string? extractedPart)
    {
        Type = file.Type;
        Path = file.Path;
        BranchName = branchName;
        ExtractedPart = extractedPart;
    }

    [Display("File type")]
    public string Type { get; set; }

    [Display("File path")]
    public string Path { get; set; }

    [Display("Branch name")]
    public string BranchName { get; set; }

    [Display("Extracted part")]
    public string? ExtractedPart { get; set; }
}
