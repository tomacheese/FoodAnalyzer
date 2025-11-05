using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// Azure の構成データを表します。
/// </summary>
internal class AzureConfigData
{
    /// <summary>
    /// Azure サービスのエンドポイント URL を取得または設定します。
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure サービスの API キーを取得または設定します。
    /// </summary>
    [JsonPropertyName("apikey")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure サービスのデプロイメント名を取得または設定します。
    /// </summary>
    [JsonPropertyName("deployment")]
    public string Deployment { get; set; } = string.Empty;
}
