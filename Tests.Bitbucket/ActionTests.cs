using Apps.Bitbucket.Actions;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class ActionTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
    {
        var actions = new Actions(InvocationContext);

        await actions.Action();
    }
}
