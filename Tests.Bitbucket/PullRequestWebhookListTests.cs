using Apps.Bitbucket.Models.Entities.PullRequest;
using Apps.Bitbucket.Models.Entities.User;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Webhooks;
using Apps.Bitbucket.Webhooks.Helpers;
using Apps.Bitbucket.Webhooks.Models.Payloads.PullRequest;
using Newtonsoft.Json;

namespace Tests.Bitbucket;

[TestClass]
public class PullRequestWebhookListTests
{
    [TestMethod]
    public void PullRequestMatches_TitleContains_ReturnsTrue()
    {
        var pullRequest = CreatePullRequest(title: "Sample PR");
        var filter = new OptionalPullRequestFilter { TitleContains = ["sample"] };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, null, filter);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PullRequestMatches_TitleContainsMismatch_ReturnsFalse()
    {
        var pullRequest = CreatePullRequest(title: "Sample PR");
        var filter = new OptionalPullRequestFilter { TitleContains = ["release"] };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, null, filter);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PullRequestMatches_TitleDoesntContainBlocks_ReturnsFalse()
    {
        var pullRequest = CreatePullRequest(title: "Draft release PR");
        var filter = new OptionalPullRequestFilter { TitleDoesntContain = ["release"] };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, null, filter);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PullRequestMatches_DescriptionContains_ReturnsTrue()
    {
        var pullRequest = CreatePullRequest(description: "Update localization files");
        var filter = new OptionalPullRequestFilter { DescriptionContains = ["localization"] };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, null, filter);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PullRequestMatches_DescriptionContainsMismatch_ReturnsFalse()
    {
        var pullRequest = CreatePullRequest(description: "Update localization files");
        var filter = new OptionalPullRequestFilter { DescriptionContains = ["billing"] };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, null, filter);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PullRequestMatches_DescriptionDoesntContainBlocks_ReturnsFalse()
    {
        var pullRequest = CreatePullRequest(description: "Do not merge yet");
        var filter = new OptionalPullRequestFilter { DescriptionDoesntContain = ["merge"] };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, null, filter);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PullRequestMatches_CombinedFiltersAllPass_ReturnsTrue()
    {
        var pullRequest = CreatePullRequest(
            title: "Sample PR",
            description: "Update localization files",
            sourceBranchName: "feature/demoing");
        var branchFilter = new OptionalBranchIdentifier { BranchNameContains = ["feature/"] };
        var pullRequestFilter = new OptionalPullRequestFilter
        {
            TitleContains = ["sample"],
            TitleDoesntContain = ["wip"],
            DescriptionContains = ["localization"],
            DescriptionDoesntContain = ["blocked"]
        };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, branchFilter, pullRequestFilter);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PullRequestMatches_SourceBranchMatches_ReturnsTrue()
    {
        var pullRequest = CreatePullRequest(sourceBranchName: "feature/demoing");
        var branchFilter = new OptionalBranchIdentifier { BranchName = "FEATURE/DEMOING" };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, branchFilter, null);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PullRequestMatches_SourceBranchMismatch_ReturnsFalse()
    {
        var pullRequest = CreatePullRequest(sourceBranchName: "feature/demoing");
        var branchFilter = new OptionalBranchIdentifier { BranchName = "main" };

        var result = PullRequestWebhookList.PullRequestMatches(pullRequest, branchFilter, null);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void PullRequestResponse_FromPayload_MapsPullRequestFields()
    {
        var payload = JsonConvert.DeserializeObject<PullRequestPayload>(ReadInputFile("pull_request_created.json"));

        Assert.IsNotNull(payload);

        Assert.IsNotNull(payload.PullRequest);

        var response = new Apps.Bitbucket.Models.Response.PullRequest.PullRequestResponse(payload.PullRequest);

        Assert.AreEqual("1", response.Id);
        Assert.AreEqual("Sample PR", response.Title);
        StringAssert.Contains(response.Description, "DM-104 Sample commit");
        Assert.AreEqual("OPEN", response.State);
        Assert.AreEqual("{56656c51-0181-4996-bf4c-795e98800820}", response.AuthorUuid);
        Assert.AreEqual("feature/demoing", response.SourceBranchName);
        Assert.AreEqual("main", response.TargetBranchName);
        Assert.AreEqual("5587a6d187df", response.SourceCommitHash);
        Assert.AreEqual("b02bcf3c149a", response.TargetCommitHash);
        Assert.AreEqual("https://bitbucket.org/bb-io-demo/feature-branch-demo/pull-requests/1", response.Url);
    }

    [TestMethod]
    public void CreateFileResponses_FilePathPatternFiltersAffectedFiles()
    {
        var diffstats = new[]
        {
            new Apps.Bitbucket.Models.Entities.Diffstat.DiffstatEntity
            {
                Status = "modified",
                NewFile = new Apps.Bitbucket.Models.Entities.File.FileEntity
                {
                    Type = "commit_file",
                    Path = "locales/en-US.json"
                }
            },
            new Apps.Bitbucket.Models.Entities.Diffstat.DiffstatEntity
            {
                Status = "modified",
                NewFile = new Apps.Bitbucket.Models.Entities.File.FileEntity
                {
                    Type = "commit_file",
                    Path = "README.md"
                }
            }
        };

        var result = WebhookFilterHelper.CreateFileResponses(
            diffstats,
            null,
            ["locales/([a-zA-Z]{2}-[a-zA-Z]{2}).json"],
            "feature/demoing");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("locales/en-US.json", result[0].Path);
        Assert.AreEqual("en-US", result[0].ExtractedPart);
    }

    private static PullRequestEntity CreatePullRequest(
        string title = "Sample PR",
        string? description = "Sample description",
        string sourceBranchName = "feature/demoing")
    {
        return new PullRequestEntity
        {
            Id = "1",
            Title = title,
            Description = description,
            State = "OPEN",
            Author = new UserEntity { Uuid = "user-id" },
            Source = new PullRequestBranchReference
            {
                Branch = new PullRequestBranch { Name = sourceBranchName },
                Commit = new PullRequestCommit { Hash = "source-hash" }
            },
            Destination = new PullRequestBranchReference
            {
                Branch = new PullRequestBranch { Name = "main" },
                Commit = new PullRequestCommit { Hash = "target-hash" }
            }
        };
    }

    private static string ReadInputFile(string fileName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName;

        return File.ReadAllText($"{projectDirectory}/TestFiles/Input/{fileName}");
    }
}
