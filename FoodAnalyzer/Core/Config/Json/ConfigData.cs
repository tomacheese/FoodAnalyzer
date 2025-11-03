using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;
internal class ConfigData
{
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    [JsonPropertyName("discord")]
    public DiscordConfigData Discord { get; set; } = new();

    [JsonPropertyName("azure")]
    public AzureConfigData Azure { get; set; } = new();
}
