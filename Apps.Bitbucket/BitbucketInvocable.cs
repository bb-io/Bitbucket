using Apps.Bitbucket.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket;

public class BitbucketInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds => 
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected BitbucketClient Client { get; }
    protected BitbucketWebClient WebClient { get; }

    protected BitbucketInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
        WebClient = new(Creds);
    }
}