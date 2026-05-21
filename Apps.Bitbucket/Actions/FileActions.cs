using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Response.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Bitbucket.Actions;

[ActionList("Files")]
public class FileActions(InvocationContext context, IFileManagementClient fileManagementClient) 
    : BitbucketInvocable(context)
{
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-commit-path-get
    [Action("Download file", Description = "Download a file")]
    public async Task<FileResponse> DownloadFile(
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] FilePathIdentifier filePathIdentifier)
    {
        string endpoint = $"repositories/{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}" +
                          $"/src/{branchIdentifier.GetBranchName()}/{filePathIdentifier.FilePath}";
        var request = new BitbucketCloudRequest(endpoint);

        var response = await Client.ExecuteWithErrorHandling(request);
        using var stream = new MemoryStream(response.RawBytes ?? []);
        var fileReference = await fileManagementClient.UploadAsync(
            stream,
            response.ContentType ?? "application/octet-stream",
            response.GetFilenameFromDispositionHeader(filePathIdentifier.FilePath));

        return new(fileReference);
    }

    [Action("Download repository as zip", Description = "Download repository content as a zip file")]
    public async Task<FileResponse> DownloadRepositoryZip(
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier)
    {
        string fileName = $"{branchIdentifier.GetBranchName()}.zip";
        string endpoint = $"{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}/get/{fileName}";
        var request = new RestRequest(endpoint);
        
        var response = await WebClient.ExecuteWithErrorHandling(request);
        using var stream = new MemoryStream(response.RawBytes ?? []);
        var fileReference = await fileManagementClient.UploadAsync(
            stream, 
            response.ContentType ?? "application/octet-stream", 
            response.GetFilenameFromDispositionHeader(fileName));

        return new(fileReference);
    }
}