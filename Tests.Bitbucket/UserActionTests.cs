using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class UserActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections(ConnectionTypes.ApiToken)]
    public async Task GetMyUserData_ReturnsCurrentUser(InvocationContext invocationContext)
    {
        var actions = new UserActions(invocationContext);

        var result = await actions.GetMyUser();

        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task GetUser_ReturnsUser(InvocationContext invocationContext)
    {
        var actions = new UserActions(invocationContext);
        var workspaceId = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var userIdentifier = new UserIdentifier { UserUuid = "{85296878-f3d9-4b77-be2a-dbfdfd8792e8}" };

        var result = await actions.GetUser(userIdentifier, workspaceId);

        PrintResult(result);
        Assert.IsNotNull(result);
    }
}