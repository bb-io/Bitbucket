using Apps.Bitbucket.Connections;
using Apps.Bitbucket.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class ConnectionValidatorTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task ValidateConnection_ValidData_ShouldBeSuccessful(InvocationContext invocationContext)
    {
        // Arrange
        var validator = new ConnectionValidator(invocationContext);
        var credentials = invocationContext.AuthenticationCredentialsProviders
            .Select(x => new AuthenticationCredentialsProvider(x.KeyName, x.Value));

        // Act
        var result = await validator.ValidateConnection(credentials, CancellationToken.None);

        // Assert
        TestContext?.WriteLine(result.Message);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod, TargetConnections]
    public async Task ValidateConnection_InvalidData_ShouldFail(InvocationContext invocationContext)
    {
        // Arrange
        var validator = new ConnectionValidator(invocationContext);
    
        var newCredentials = invocationContext.AuthenticationCredentialsProviders
            .Select(x => new AuthenticationCredentialsProvider(
                x.KeyName,
                x.KeyName == CredsNames.ConnectionType ? x.Value : x.Value + "_incorrect"));
    
        // Act
        var result = await validator.ValidateConnection(newCredentials, CancellationToken.None);
    
        // Assert
        TestContext?.WriteLine(result.Message);
        Assert.IsFalse(result.IsValid);
    }
}