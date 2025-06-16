interface IProvider
{
    ProviderType Type { get; }
    List<string> RequiredInputs { get; }
    List<string> ResolvableInputs { get; }

    // Method that returns the result object
    object ExecuteAndGetResult(Dictionary<string, object> inputs);

    // Method to extract specific values
    object? ExtractInput(object result, string key);

    // Default implementation that handles the common extraction logic
    object? Execute(ref Dictionary<string, object> inputs)
    {
        var result = ExecuteAndGetResult(inputs);

        foreach (var key in ResolvableInputs)
        {
            var value = ExtractInput(result, key);
            if (value != null)
            {
                inputs[key] = value;
            }
        }

        return result;
    }
}
