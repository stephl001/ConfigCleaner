using System.Reflection;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit.Categories;

namespace ConfigCleaner.Tests;

[UnitTest]
public class UnitTest1
{
    private static readonly Assembly TestAssembly = typeof(UnitTest1).Assembly;
    
    [Theory, MemberData(nameof(TestData))]
    public void Test1(IConfiguration baseConfiguration, string sourceConfigPath, string targetConfigPath)
    {
        // Arrange
        JsonDuplicateProcessor processor = new();
        Utf8JsonReader sourceConfig = ReadJsonResource(sourceConfigPath);
        using MemoryStream outputStream = new MemoryStream();
        Utf8JsonWriter targetWriter = new Utf8JsonWriter(outputStream);
        using Stream expectedConfigStream = TestAssembly.GetManifestResourceStream(targetConfigPath)!;
        IConfiguration expectedConfig = BuildConfiguration(expectedConfigStream);
        
        // Act
        processor.Process(baseConfiguration, sourceConfig, targetWriter);

        // Assert
        outputStream.Seek(0L, SeekOrigin.Begin);
        IConfiguration processedConfig = BuildConfiguration(outputStream);
        ConfigurationsEqual(processedConfig, expectedConfig).Should().BeTrue();
    }
    
    public static IEnumerable<object[]> TestData()
    {
        using Stream baseConfigStream = TestAssembly.GetManifestResourceStream(typeof(UnitTest1), "Configs.appsettings.json")!;
        var baseConfiguration = new ConfigurationBuilder()
            .AddJsonStream(baseConfigStream)
            .Build();

        foreach (string resourceName in TestAssembly.GetManifestResourceNames().Where(name => name.Contains(".Configs.Source.")))
            yield return [baseConfiguration, resourceName, resourceName.Replace("Source", "Expected")];
    }


    private bool ConfigurationsEqual(IConfiguration config1, IConfiguration config2)
    {
        foreach (IConfigurationSection section in config1.GetChildren())
        {
            if (section.GetChildren().Any())
            {
                if (!ConfigurationsEqual(section, config2))
                    return false;
            }
            else if (section.Value != config2[section.Path])
                return false;
        }

        return true;
    }

    private IConfigurationRoot BuildConfiguration(Stream jsonStream)
    {
        ConfigurationBuilder b = new();
        b.AddJsonStream(jsonStream);
        return b.Build();
    }

    private Utf8JsonReader ReadJsonResource(string path)
    {
        using Stream resourceStream = TestAssembly.GetManifestResourceStream(path)!;
        using StreamReader reader = new StreamReader(resourceStream);
        string json = reader.ReadToEnd();
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        return new Utf8JsonReader(buffer);
    }
}