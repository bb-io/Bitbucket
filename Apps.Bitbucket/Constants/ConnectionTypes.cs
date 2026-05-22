namespace Apps.Bitbucket.Constants;

public static class ConnectionTypes
{
    public const string ApiToken = "API Token";
    public const string OAuth2 = "OAuth2";

    public static readonly string[] SupportedConnectionTypes = [ApiToken, OAuth2];
}