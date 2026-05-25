namespace Apps.Bitbucket.Extensions;

public static class DictionaryExtensions
{
    public static string ToLogString(this Dictionary<string, string> dictionary)
    {
        return string.Join(", ", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"));
    }
}