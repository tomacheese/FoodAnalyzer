using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// モニター設定データ全体
/// </summary>
internal class MonitorData
{
    /// <summary>
    /// 設定ファイルのバージョン
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// モニター対象のチャンネル情報リスト
    /// </summary>
    [JsonPropertyName("channels")]
    public List<MonitorChannelData> Channels { get; set; } = [];
}
