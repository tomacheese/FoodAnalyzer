using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// アプリケーションの設定データ
/// </summary>
internal class ConfigData
{
    /// <summary>
    /// 設定ファイルのバージョン
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// Discord 関連の設定データ
    /// </summary>
    [JsonPropertyName("discord")]
    public DiscordConfigData Discord { get; set; } = new();

    /// <summary>
    /// Azure 関連の設定データ
    /// </summary>
    [JsonPropertyName("azure")]
    public AzureConfigData Azure { get; set; } = new();
}
