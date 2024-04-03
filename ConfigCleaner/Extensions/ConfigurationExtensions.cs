using Microsoft.Extensions.Configuration;

namespace ConfigCleaner.Extensions;

public static class ConfigurationExtensions
{
    public static Dictionary<string, string?> ToDictionary(this IConfiguration configuration)
    {
        var keyValues = new Dictionary<string, string?>();
        foreach ((string key, string? value) in configuration.AsEnumerable())
        {
            keyValues.Add(key, value);
        }

        return keyValues;
    }
}