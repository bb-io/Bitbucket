using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Bitbucket.Webhooks.Handlers;

public class PullRequestCreatedOrUpdatedEventHandler(
    InvocationContext context,
    [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
    [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier)
    : BaseEventHandler(context, workspaceIdentifier.WorkspaceUuid, repositoryIdentifier.RepositoryUuid)
{
    protected override IEnumerable<string> EventNames => ["pullrequest:created", "pullrequest:updated"];
}
