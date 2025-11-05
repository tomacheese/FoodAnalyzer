using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// モニター設定データ全体を表します。
/// </summary>
internal class MonitorData
{
    /// <summary>
    /// 設定ファイルのバージョンを取得または設定します。
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// モニター対象のチャンネル情報リストを取得または設定します。
    /// </summary>
    [JsonPropertyName("channels")]
    public List<MonitorChannelData> Channels { get; set; } = [];
}
