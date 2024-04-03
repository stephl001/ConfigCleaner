using ConfigCleaner.Extensions;
using Microsoft.Extensions.Configuration;

namespace ConfigCleaner;

public sealed class ConfigurationReducer
{
    public IConfiguration Reduce(IConfiguration baseConfiguration, IConfiguration specificConfiguration)
    {
        var builder = new ConfigurationBuilder();

        // Get the key-value pairs from the specific configuration
        Dictionary<string, string?> allProps = specificConfiguration.ToDictionary();

        // Add only those properties from specificConfiguration that have different values than baseConfiguration
        bool identical = true;
        var specificProperties = new Dictionary<string, string?>();
        foreach ((string key, string? specificValue) in allProps)
        {
            if (baseConfiguration[key] == specificValue)
            {
                identical = false;
                continue;
            }
            
            specificProperties.Add(key, specificValue);
        }

        if (identical)
            return specificConfiguration;

        // Build the resulting configuration only if different from original specific configuration
        return builder.AddInMemoryCollection(specificProperties).Build();
    }
}