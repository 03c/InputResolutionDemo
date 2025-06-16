using System.Text.Json;

var initialInputs = new Dictionary<string, object> { { "tcode", "T0000000000001" } };
var requestedProviders = new List<ProviderType>
{
    ProviderType.CheckICloud,
    ProviderType.GetDeviceInfo,
};

try
{
    var result = ProviderController.PerformDiagnosticsTests(initialInputs, requestedProviders);

    Console.WriteLine("=== Provider Execution Results ===\n");

    foreach (var kvp in result)
    {
        Console.WriteLine($"{kvp.Key}:");
        var json = JsonSerializer.Serialize(
            kvp.Value,
            new JsonSerializerOptions { WriteIndented = true }
        );
        Console.WriteLine($"{json}\n");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
