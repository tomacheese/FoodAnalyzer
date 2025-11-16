using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// Azure の構成データ
/// </summary>
internal class AzureConfigData
{
    /// <summary>
    /// Azure サービスのエンドポイント URL
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure サービスの API キー
    /// </summary>
    [JsonPropertyName("apikey")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure サービスのデプロイメント名
    /// </summary>
    [JsonPropertyName("deployment")]
    public string Deployment { get; set; } = string.Empty;
}
