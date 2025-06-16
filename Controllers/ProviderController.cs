public static class ProviderController
{
    private static readonly Dictionary<ProviderType, IProvider> _providers = new()
    {
        { ProviderType.CheckICloud, new CheckICloud() },
        { ProviderType.GetDeviceInfo, new GetDeviceInfo() },
    };

    public static Dictionary<ProviderType, object> PerformDiagnosticsTests(
        Dictionary<string, object> inputs,
        List<ProviderType> requestedProviders
    )
    {
        var providers = requestedProviders.Select(type => _providers[type]).ToList();
        var orderedProviders = ResolveProviderOrder(providers, inputs);

        Console.WriteLine("=== Provider Execution Order ===");
        for (int i = 0; i < orderedProviders.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {orderedProviders[i].Type}");
        }
        Console.WriteLine();

        var currentInputs = new Dictionary<string, object>(inputs);
        var providerResults = new Dictionary<ProviderType, object>();

        foreach (var provider in orderedProviders)
        {
            Console.WriteLine($"=== Executing {provider.Type} ===");
            Console.WriteLine("Available inputs:");
            foreach (var input in currentInputs)
            {
                Console.WriteLine($"  {input.Key}: {input.Value}");
            }
            Console.WriteLine();

            var result = provider.Execute(ref currentInputs);
            if (result != null)
            {
                providerResults[provider.Type] = result;
            }
        }

        return providerResults;
    }

    private static List<IProvider> ResolveProviderOrder(
        List<IProvider> providers,
        Dictionary<string, object> inputs
    )
    {
        var available = inputs.Keys.ToHashSet();
        var result = new List<IProvider>();
        var remaining = new List<IProvider>(providers);

        while (remaining.Count > 0)
        {
            var ready =
                remaining.FirstOrDefault(p => p.RequiredInputs.All(available.Contains))
                ?? throw new InvalidOperationException(
                    "Circular dependency detected or missing required inputs."
                );

            result.Add(ready);
            available.UnionWith(ready.ResolvableInputs);
            remaining.Remove(ready);
        }
        return result;
    }
}
