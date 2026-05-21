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

    public CloudQueryBuilder In(string field, List<string>? values, bool useQuotes = true)
    {
        if (values == null || values.Count == 0) 
            return this;

        var validValues = values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
        if (validValues.Count == 0) 
            return this;

        var orConditions = validValues.Select(v => 
        {
            if (!useQuotes) 
                return $"{field}={v}";
            
            var safeValue = v.Replace("\"", "\\\"");
            return $"{field}=\"{safeValue}\"";
        });

        string combinedGroup = string.Join(" OR ", orConditions);
        _conditions.Add($"({combinedGroup})");
    
        return this;
    }
    
    public bool HasConditions => _conditions.Count > 0;

    public override string ToString() => string.Join(" AND ", _conditions);
}