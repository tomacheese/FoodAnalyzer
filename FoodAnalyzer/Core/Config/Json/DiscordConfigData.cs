using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// Discord の設定データを表します。
/// </summary>
internal class DiscordConfigData
{
    /// <summary>
    /// Discord ボットのトークンを取得または設定します。
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
