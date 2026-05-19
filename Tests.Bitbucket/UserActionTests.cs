using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class UserActionTests : TestBase
{
    [TestMethod]
    public async Task GetMyUserData_ReturnsCurrentUser()
    {
        // Arrange
        var actions = new UserActions(InvocationContext);

        // Act
        var result = await actions.GetMyUser();

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetUser_ReturnsUser()
    {
        // Arrange
        var actions = new UserActions(InvocationContext);
        var workspaceId = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var userIdentifier = new UserIdentifier { UserUuid = "{85296878-f3d9-4b77-be2a-dbfdfd8792e8}" };

        // Act
        var result = await actions.GetUser(userIdentifier, workspaceId);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}
