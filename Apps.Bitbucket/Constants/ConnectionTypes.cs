namespace Apps.Bitbucket.Constants;

public static class ConnectionTypes
{
    public const string ApiToken = "API Token";
    public const string OAuth2 = "OAuth2";
    public const string RepoAccessToken = "Repository access token";

    public static readonly string[] SupportedConnectionTypes = [ApiToken, OAuth2, RepoAccessToken];
}