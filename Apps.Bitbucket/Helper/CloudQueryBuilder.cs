namespace Apps.Bitbucket.Helper;

public class CloudQueryBuilder
{
    private readonly List<string> _conditions = [];

    public CloudQueryBuilder AddCondition(string field, string op, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            return this;

        var safeValue = value.Replace("\"", "\\\"");
        
        _conditions.Add($"{field}{op}\"{safeValue}\"");
        return this;
    }

    public CloudQueryBuilder Contains(string field, string? value) => AddCondition(field, "~", value);
    public CloudQueryBuilder EqualsExact(string field, string? value) => AddCondition(field, "=", value);

    public bool HasConditions => _conditions.Count > 0;

    public override string ToString() => string.Join(" AND ", _conditions);
}