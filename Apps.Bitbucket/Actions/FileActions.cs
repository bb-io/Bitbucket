using System.Net;
using System.Net.Mime;
using System.Text;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Request.File;
using Apps.Bitbucket.Models.Response.File;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using RestSharp;

namespace Apps.Bitbucket.Actions;

[ActionList("Files")]
public class FileActions(InvocationContext context, IFileManagementClient fileManagementClient) 
    : BitbucketInvocable(context)
{
    private readonly IdentifierResolver _resolver = new(context.AuthenticationCredentialsProviders);
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-commit-path-get
    [Action("Download file", Description = "Download a file")]
    public async Task<FileReferenceResponse> DownloadFile(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] FilePathIdentifier filePathIdentifier,
        [ActionParameter] DownloadFileRequest downloadInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        string sourceRef = await branchIdentifier.ResolveSourceRefAsync(Client, workspaceUuid, repositoryUuid);
        
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/src/{sourceRef}/{filePathIdentifier.FilePath}";
        var request = new BitbucketCloudRequest(endpoint);

        var response = await Client.ExecuteWithErrorHandling(request);
        using var stream = new MemoryStream(response.RawBytes ?? []);

        string fileName = response.GetFilenameFromDispositionHeader(filePathIdentifier.FilePath);
        if (!MimeTypes.TryGetMimeType(fileName, out var mimeType))
            mimeType = MediaTypeNames.Application.Octet;
        
        var fileResult = Transformation.Load(stream, fileName, mimeType).Source();
        if (!fileResult.Success)
        {
            var directFileReference = await fileManagementClient.UploadAsync(stream, mimeType, fileName);
            InvocationContext.Logger?.LogInformation($"Not a Blackbird interoperable file: {fileResult.Error}", []);
            return new(directFileReference);
        }

        var fileContent = fileResult.Value;
        string branchName = branchIdentifier.GetBranchName();
        
        fileContent.Language = downloadInput.SourceLanguage;
        fileContent.SystemReference.ContentId = downloadInput.ContentId;
        fileContent.SystemReference.AddMetadata(workspaceUuid, repositoryUuid, branchName, filePathIdentifier.FilePath, fileName);
       
        var fileReference = await fileManagementClient.UploadAsync(fileContent.ToStream(), mimeType, fileName);
        return new(fileReference);
    }
    
    [Action("Download repository as zip", Description = "Download repository content as a zip file")]
    public async Task<FileReferenceResponse> DownloadRepositoryZip(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        string fileName = $"{branchIdentifier.GetBranchName()}.zip";
        var request = new RestRequest($"{workspaceUuid}/{repositoryUuid}/get/{fileName}");
        
        var response = await WebClient.ExecuteWithErrorHandling(request);
        using var stream = new MemoryStream(response.RawBytes ?? []);
        var fileReference = await fileManagementClient.UploadAsync(
            stream, 
            response.ContentType ?? "application/octet-stream", 
            response.GetFilenameFromDispositionHeader(fileName));

        return new(fileReference);
    }
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-post
    [Action("Delete file", Description = "Commit file deletion")]
    public async Task DeleteFile(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] FilePathIdentifier filePathIdentifier,
        [ActionParameter] DeleteFileRequest deleteInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/src";
        var request = new BitbucketCloudRequest(endpoint, Method.Post) { AlwaysMultipartFormData = true }
            .AddParameter("files", filePathIdentifier.FilePath)
            .AddParameterIfNotEmpty("branch", branchIdentifier.BranchName)
            .AddParameterIfNotEmpty("message", deleteInput.Message);

        await Client.ExecuteWithErrorHandling(request);
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-commit-path-get
    [Action("File exists", Description = "Check if file exists by path")]
    public async Task<bool> FileExists(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] FilePathIdentifier filePathIdentifier)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        string sourceRef = await branchIdentifier.ResolveSourceRefAsync(Client, workspaceUuid, repositoryUuid);
        
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}" +
                          $"/src/{sourceRef}/{filePathIdentifier.FilePath}";
        var request = new BitbucketCloudRequest(endpoint)
            .AddQueryParameter("format", "meta");

        var response = await Client.ExecuteWithExpectedStatuses(request, HttpStatusCode.NotFound);
        return response.IsSuccessful;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-post
    [Action("Upload file", Description = "Commit file upload. Overwrites existing file")]
    public async Task<FileReferenceResponse> UploadFile(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] OptionalFolderPathIdentifier optionalFolderPathIdentifier,
        [ActionParameter] UploadFileRequest uploadInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        var file = await fileManagementClient.DownloadAsync(uploadInput.File);
        var transformationResult = Transformation.Load(file, uploadInput.File.Name, uploadInput.File.ContentType);
        string content = transformationResult.ReadContent(file, InvocationContext.Logger);
        
        var fileBytes = Encoding.UTF8.GetBytes(content);
        string fileName = uploadInput.FileName ?? uploadInput.File.Name;
        string? folderPath = optionalFolderPathIdentifier.FolderPath;
        string targetFilePath = string.IsNullOrEmpty(folderPath) ? fileName : $"{folderPath}/{fileName}";
        
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/src";
        var request = new BitbucketCloudRequest(endpoint, Method.Post) { AlwaysMultipartFormData = true }
            .AddParameterIfNotEmpty("branch", branchIdentifier.BranchName)
            .AddParameterIfNotEmpty("message", uploadInput.Message)
            .AddFile(targetFilePath, fileBytes, fileName, uploadInput.File.ContentType);

        await Client.ExecuteWithErrorHandling(request);
        
        if (!transformationResult.Success) 
            return new(uploadInput.File);
        
        string branchName = branchIdentifier.GetBranchName();
        
        var transformation = transformationResult.Value;
        transformation.TargetSystemReference.AddMetadata(workspaceUuid, repositoryUuid, branchName, targetFilePath, fileName);

        var fileData = transformationResult.ToResultFile();
        var uploadedFile = await fileManagementClient.UploadAsync(fileData.Stream, fileData.MediaType, fileData.FileName);
        return new(uploadedFile);
    }
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-commit-path-get
    [Action("Search files in folder", Description = "Search files in a folder")]
    public async Task<SearchFilesResponse> SearchFiles(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier,
        [ActionParameter] OptionalFolderPathIdentifier optionalFolderPathIdentifier,
        [ActionParameter] SearchFilesRequest searchInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        string rawPath = optionalFolderPathIdentifier.FolderPath?.Trim('/') ?? string.Empty;
        string safeFolderPath = string.IsNullOrEmpty(rawPath) ? string.Empty : $"{rawPath}/";
        string sourceRef = await branchIdentifier.ResolveSourceRefAsync(Client, workspaceUuid, repositoryUuid);
        
        string baseEndpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/src/{sourceRef}/";
        var request = new BitbucketCloudRequest(baseEndpoint + safeFolderPath)
            .AddBitbucketQuery(q =>
            {
                q.EqualsExact("type", "commit_file");
                q.Contains("path", searchInput.PathNameContains);
            });
        
        if (searchInput.IncludeSubfolders == true)
            request.AddQueryParameter("max_depth", searchInput.MaxDepth ?? 5); 
        
        var response = await Client.Paginate<FileEntity>(request);
        return new(response.Select(x => new FileResponse(x)).ToList());
    }
}
