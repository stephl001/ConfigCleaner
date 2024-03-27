using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ConfigCleaner;

public sealed class JsonDuplicateProcessor
{
    public void Process(IConfiguration baseConfiguration, Utf8JsonReader sourceConfig, Utf8JsonWriter targetConfig)
    {
        while (sourceConfig.Read())
        {
            switch (sourceConfig.TokenType)
            {
                case JsonTokenType.StartObject:
                    targetConfig.WriteStartObject();
                    break;
                case JsonTokenType.EndObject:
                    targetConfig.WriteEndObject();
                    break;
                case JsonTokenType.StartArray:
                    targetConfig.WriteStartArray();
                    break;
                case JsonTokenType.EndArray:
                    targetConfig.WriteEndArray();
                    break;
                case JsonTokenType.PropertyName:
                    targetConfig.WritePropertyName(sourceConfig.GetString()!);
                    break;
                case JsonTokenType.String:
                    targetConfig.WriteStringValue(sourceConfig.GetString());
                    break;
                case JsonTokenType.Number:
                    if (sourceConfig.TryGetDecimal(out decimal num))
                        targetConfig.WriteNumberValue(num);
                    break;
                case JsonTokenType.True:
                    targetConfig.WriteBooleanValue(true);
                    break;
                case JsonTokenType.False:
                    targetConfig.WriteBooleanValue(false);
                    break;
                case JsonTokenType.Null:
                    targetConfig.WriteNullValue();
                    break;
            }
        }

        targetConfig.Flush();
    }

}