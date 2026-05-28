using Apps.Bitbucket.Models.Entities.File;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Bitbucket.Models.Response.File;

public record FileResponse
{
    public FileResponse(FileEntity file)
    {
        Type = file.Type;
        Path = file.Path;
    }

    [Display("File type")] 
    public string Type { get; set; }
    
    [Display("File path")]
    public string Path { get; set; }
}