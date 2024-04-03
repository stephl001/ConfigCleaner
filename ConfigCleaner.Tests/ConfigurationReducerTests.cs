using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit.Categories;

namespace ConfigCleaner.Tests
{
    [UnitTest]
    public sealed class ConfigurationReducerTests
    {
        [Fact]
        public void Reduce_Should_Return_SpecificConfiguration_When_Identical()
        {
            // Arrange
            var baseConfiguration = new ConfigurationBuilder().Build();
            var specificConfiguration = new ConfigurationBuilder().Build();
            var reducer = new ConfigurationReducer();

            // Act
            var result = reducer.Reduce(baseConfiguration, specificConfiguration);

            // Assert
            result.Should().Be(specificConfiguration);
        }

        [Fact]
        public void Reduce_Should_Remove_Identical_Properties_From_SpecificConfiguration()
        {
            // Arrange
            var baseConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" }
                })
                .Build();

            var specificConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Key1", "Value1" }, // Identical property
                    { "Key3", "Value3" }  // Different property
                })
                .Build();

            var reducer = new ConfigurationReducer();

            // Act
            var result = reducer.Reduce(baseConfiguration, specificConfiguration);

            // Assert
            result["Key3"].Should().Be("Value3");
            result.GetSection("Key1").Exists().Should().BeFalse(); // Key1 should be removed
        }
    }
}