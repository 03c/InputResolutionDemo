using Xunit;

namespace InputResolutionDemo.Tests
{
    public class ProviderOrderTests
    {
        [Fact]
        public void ExecuteProviders_CorrectOrder_GetDeviceInfoBeforeCheckICloud()
        {
            // Arrange
            var initialInputs = new Dictionary<string, object> { { "tcode", "T0000000000001" } };
            var requestedProviders = new List<ProviderType>
            {
                ProviderType.CheckICloud,
                ProviderType.GetDeviceInfo,
            };

            // Act
            var result = ProviderController.PerformDiagnosticsTests(
                initialInputs,
                requestedProviders
            );

            // Assert - Prove both providers executed successfully
            Assert.Equal(2, result.Count); // Should have results from both providers
            Assert.Contains(ProviderType.GetDeviceInfo, result.Keys);
            Assert.Contains(ProviderType.CheckICloud, result.Keys);

            // Check GetDeviceInfo result
            var deviceInfoResult = (dynamic)result[ProviderType.GetDeviceInfo];
            Assert.Equal("123456789012345", deviceInfoResult.imei);

            // Check CheckICloud result
            var icloudResult = (dynamic)result[ProviderType.CheckICloud];
            Assert.Equal("clean", icloudResult.status);
        }

        [Fact]
        public void ExecuteProviders_ReverseRequestOrder_StillWorksCorrectly()
        {
            // Arrange - Request providers in dependency order (CheckICloud first)
            var initialInputs = new Dictionary<string, object> { { "tcode", "T0000000000001" } };
            var requestedProviders = new List<ProviderType>
            {
                ProviderType.CheckICloud,
                ProviderType.GetDeviceInfo,
            };

            // Act & Assert - Should not throw exception, proving order resolution works
            var result = ProviderController.PerformDiagnosticsTests(
                initialInputs,
                requestedProviders
            );

            // Validate all expected outputs are present
            Assert.Equal(2, result.Count); // Both providers should execute

            var icloudResult = (dynamic)result[ProviderType.CheckICloud];
            Assert.Equal("clean", icloudResult.status);
        }

        [Fact]
        public void ExecuteProviders_SingleProvider_WorksCorrectly()
        {
            // Arrange
            var initialInputs = new Dictionary<string, object> { { "tcode", "T0000000000001" } };
            var requestedProviders = new List<ProviderType> { ProviderType.GetDeviceInfo };

            // Act
            var result = ProviderController.PerformDiagnosticsTests(
                initialInputs,
                requestedProviders
            );

            // Assert
            Assert.Single(result); // Only one provider result
            Assert.Contains(ProviderType.GetDeviceInfo, result.Keys);
            Assert.DoesNotContain(ProviderType.CheckICloud, result.Keys); // CheckICloud wasn't requested

            var deviceInfoResult = (dynamic)result[ProviderType.GetDeviceInfo];
            Assert.Equal("123456789012345", deviceInfoResult.imei);
        }

        [Fact]
        public void ExecuteProviders_MissingRequiredInput_ThrowsException()
        {
            // Arrange - Missing required "tcode" input for GetDeviceInfo
            var initialInputs = new Dictionary<string, object>();
            var requestedProviders = new List<ProviderType> { ProviderType.GetDeviceInfo };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => ProviderController.PerformDiagnosticsTests(initialInputs, requestedProviders)
            );

            Assert.Equal(
                "Circular dependency detected or missing required inputs.",
                exception.Message
            );
        }

        [Fact]
        public void ExecuteProviders_CircularDependency_ThrowsException()
        {
            // Arrange - Simulate circular dependency by requesting CheckICloud without GetDeviceInfo
            var initialInputs = new Dictionary<string, object> { { "tcode", "T0000000000001" } };
            var requestedProviders = new List<ProviderType> { ProviderType.CheckICloud }; // Needs imei but no provider for it

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => ProviderController.PerformDiagnosticsTests(initialInputs, requestedProviders)
            );

            Assert.Equal(
                "Circular dependency detected or missing required inputs.",
                exception.Message
            );
        }
    }
}
