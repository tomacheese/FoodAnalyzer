using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;
internal class DiscordConfigData
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
