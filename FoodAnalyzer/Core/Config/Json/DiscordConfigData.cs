using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// Discord の設定データ
/// </summary>
internal class DiscordConfigData
{
    /// <summary>
    /// Discord ボットのトークン
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
