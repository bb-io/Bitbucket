using System.Linq.Expressions;
using System.Reflection;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Bitbucket.Helper;

public static class InputValidator
{
    public static void ThrowIfMissing(params Expression<Func<string?>>[] propertyExpressions)
    {
        var missingInputs = new List<string>();

        foreach (var expr in propertyExpressions)
        {
            var value = expr.Compile().Invoke();
            if (!string.IsNullOrWhiteSpace(value)) 
                continue;
            
            if (expr.Body is MemberExpression memberExpr && memberExpr.Member is PropertyInfo propInfo)
            {
                var displayAttribute = propInfo.GetCustomAttribute<DisplayAttribute>();

                string displayName = displayAttribute?.Name ?? propInfo.Name;
                missingInputs.Add(displayName);
            }
            else
                missingInputs.Add("Unknown Input");
        }

        if (missingInputs.Count == 0) 
            return;
        
        string missingInputsString = string.Join(", ", missingInputs);
        throw new PluginMisconfigurationException($"Please specify these inputs first: {missingInputsString}");
    }
}