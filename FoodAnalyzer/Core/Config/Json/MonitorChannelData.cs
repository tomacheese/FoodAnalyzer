using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// データ監視用のチャンネル情報を表します。
/// </summary>
internal class MonitorChannelData
{
    /// <summary>
    /// ギルドIDを取得または設定します。
    /// </summary>
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; } = 0;

    /// <summary>
    /// 受信チャンネルIDを取得または設定します。
    /// </summary>
    [JsonPropertyName("received_channel_id")]
    public ulong ReceivedChannelId { get; set; } = 0;

    /// <summary>
    /// 送信チャンネルIDを取得または設定します。
    /// </summary>
    [JsonPropertyName("sent_channel_id")]
    public ulong SentChannelId { get; set; } = 0;
}
