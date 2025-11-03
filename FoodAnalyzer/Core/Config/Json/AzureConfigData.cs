using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;
internal class AzureConfigData
{
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("deployment")]
    public string Deployment { get; set; } = string.Empty;
}
