using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Bitbucket.Helper;

public static class InputValidator
{
    public static void ThrowIfMissing(params (string? Value, string DisplayName)[] inputs)
    {
        var missingInputs = inputs
            .Where(x => string.IsNullOrWhiteSpace(x.Value))
            .Select(x => x.DisplayName)
            .ToList();

        if (missingInputs.Count == 0) 
            return;
        
        string missingInputsString = string.Join(", ", missingInputs);
        throw new PluginMisconfigurationException($"Please specify these inputs first: {missingInputsString}");
    }
}