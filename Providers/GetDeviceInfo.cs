class GetDeviceInfo : IProvider
{
    public ProviderType Type => ProviderType.GetDeviceInfo;
    public List<string> RequiredInputs => new() { "tcode" };
    public List<string> ResolvableInputs => new() { "imei" };

    public object ExecuteAndGetResult(Dictionary<string, object> inputs)
    {
        // Simulate device lookup based on tcode
        return new
        {
            imei = "123456789012345",
            model = "iPhone 14 Pro",
            storage = "256GB",
            color = "Deep Purple",
            carrier = "Unlocked",
            purchase_date = "2023-09-15",
            warranty_status = "Active",
            serial_number = "F2LWN8K9N123",
            ios_version = "17.2.1",
            activation_status = "Activated",
            find_my_status = "Enabled",
            battery_health = "98%",
            condition = "Excellent",
        };
    }

    public object? ExtractInput(object result, string key)
    {
        var deviceInfo = (dynamic)result;
        return key switch
        {
            "imei" => deviceInfo.imei,
            _ => null,
        };
    }
}
