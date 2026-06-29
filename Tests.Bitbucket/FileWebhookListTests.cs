using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Webhooks;
using Apps.Bitbucket.Webhooks.Models.Entity.Push;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Tests.Bitbucket;

[TestClass]
public class FileWebhookListTests
{
    [TestMethod]
    public void BranchMatches_ExactBranch_ReturnsTrue()
    {
        var filter = new OptionalBranchIdentifier { BranchName = "Main" };

        var result = FilesWebhookList.BranchMatches("main", filter);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void BranchMatches_ExactBranchMismatch_ReturnsFalse()
    {
        var filter = new OptionalBranchIdentifier { BranchName = "main" };

        var result = FilesWebhookList.BranchMatches("feature/new-login", filter);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void BranchMatches_ContainsAny_ReturnsTrue()
    {
        var filter = new OptionalBranchIdentifier
        {
            BranchNameContains = ["feature/", "hotfix/"]
        };

        var result = FilesWebhookList.BranchMatches("feature/new-login", filter);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void BranchMatches_DoesntContainBlocks_ReturnsFalse()
    {
        var filter = new OptionalBranchIdentifier
        {
            BranchNameDoesntContain = ["staging", "main"]
        };

        var result = FilesWebhookList.BranchMatches("release/staging", filter);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void BranchMatches_CombinedFiltersAllPass_ReturnsTrue()
    {
        var filter = new OptionalBranchIdentifier
        {
            BranchNameContains = ["feature/"],
            BranchNameDoesntContain = ["wip"]
        };

        var result = FilesWebhookList.BranchMatches("feature/new-login", filter);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CreateFileResponse_RegexCapturesExtractedPart()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" };
        var filepath = new OptionalFilepath
        {
            FilePathPatterns = [@"/locales/([a-z]{2}_[A-Z]{2})\.json$"]
        };

        var result = FilesWebhookList.CreateFileResponse(file, filepath.FilePathPatterns, "feature/new-login");

        Assert.IsNotNull(result);
        Assert.AreEqual("/locales/en_US.json", result.Path);
        Assert.AreEqual("en_US", result.ExtractedPart);
        Assert.AreEqual("commit_file", result.Type);
        Assert.AreEqual("feature/new-login", result.BranchName);
    }

    [TestMethod]
    public void CreateFileResponse_IncludesCommitMessage()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" };

        var result = FilesWebhookList.CreateFileResponse(
            file,
            [@"/locales/([a-z]{2}_[A-Z]{2})\.json$"],
            "feature/new-login",
            "Add English locale");

        Assert.IsNotNull(result);
        Assert.AreEqual("Add English locale", result.CommitMessage);
    }

    [TestMethod]
    public void CreateFileResponse_MultipleFiles_ReturnsMatchedFilesOnly()
    {
        var files = new[]
        {
            new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" },
            new FileEntity { Type = "commit_file", Path = "/README.md" },
            new FileEntity { Type = "commit_file", Path = "/locales/es_MX.json" }
        };
        var filepath = new OptionalFilepath
        {
            FilePathPatterns = [@"/locales/([a-z]{2}_[A-Z]{2})\.json$"]
        };

        var result = files
            .Select(file => FilesWebhookList.CreateFileResponse(file, filepath.FilePathPatterns, "feature/add-banner"))
            .Where(file => file is not null)
            .Select(file => file!)
            .ToList();

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEqual(new[] { "en_US", "es_MX" }, result.Select(x => x.ExtractedPart).ToArray());
    }

    [TestMethod]
    public void CreateFileResponse_MultiplePatterns_UsesFirstMatchingCapture()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/ar_AE.json" };
        var filepath = new OptionalFilepath
        {
            FilePathPatterns =
            [
                @"/docs/(.+)\.md$",
                @"/locales/([a-z]{2}_[A-Z]{2})\.json$"
            ]
        };

        var result = FilesWebhookList.CreateFileResponse(file, filepath.FilePathPatterns, "feature/add-banner");

        Assert.IsNotNull(result);
        Assert.AreEqual("ar_AE", result.ExtractedPart);
    }

    [TestMethod]
    public void CreateFileResponse_NoPatterns_ReturnsFileWithEmptyExtractedPart()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" };

        var result = FilesWebhookList.CreateFileResponse(file, new OptionalFilepath().FilePathPatterns, "main");

        Assert.IsNotNull(result);
        Assert.AreEqual("/locales/en_US.json", result.Path);
        Assert.IsNull(result.ExtractedPart);
    }

    [TestMethod]
    public void CreateFileResponse_InvalidRegex_Throws()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" };
        var filepath = new OptionalFilepath { FilePathPatterns = ["("] };

        try
        {
            FilesWebhookList.CreateFileResponse(file, filepath.FilePathPatterns, "main");
            Assert.Fail($"Expected exception of type {nameof(PluginMisconfigurationException)}.");
        }
        catch (PluginMisconfigurationException)
        {
        }
    }

    [TestMethod]
    public void CreateFileResponse_ZeroCaptureGroups_ReturnsMatchedFileWithEmptyExtractedPart()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" };
        var filepath = new OptionalFilepath { FilePathPatterns = [@"/locales/[a-z]{2}_[A-Z]{2}\.json$"] };

        var result = FilesWebhookList.CreateFileResponse(file, filepath.FilePathPatterns, "main");

        Assert.IsNotNull(result);
        Assert.IsNull(result.ExtractedPart);
    }

    [TestMethod]
    public void CreateFileResponse_MultipleCaptureGroups_Throws()
    {
        var file = new FileEntity { Type = "commit_file", Path = "/locales/en_US.json" };
        var filepath = new OptionalFilepath { FilePathPatterns = [@"/locales/([a-z]{2})_([A-Z]{2})\.json$"] };

        try
        {
            FilesWebhookList.CreateFileResponse(file, filepath.FilePathPatterns, "main");
            Assert.Fail($"Expected exception of type {nameof(PluginMisconfigurationException)}.");
        }
        catch (PluginMisconfigurationException)
        {
        }
    }

    [TestMethod]
    public void GetCommitMessage_MultipleCommits_JoinsWithNewLines()
    {
        var change = new Change
        {
            Commits =
            [
                new PushCommit { Message = "Add English locale\n" },
                new PushCommit { Message = "Update Spanish locale" }
            ]
        };

        var result = FilesWebhookList.GetCommitMessage(change);

        Assert.AreEqual($"Add English locale{Environment.NewLine}Update Spanish locale", result);
    }
}
