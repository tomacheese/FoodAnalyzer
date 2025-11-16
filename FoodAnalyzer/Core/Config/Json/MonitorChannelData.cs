using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// データ監視用のチャンネル情報
/// </summary>
internal class MonitorChannelData
{
    /// <summary>
    /// ギルドID
    /// </summary>
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; } = 0;

    /// <summary>
    /// 受信チャンネルID
    /// </summary>
    [JsonPropertyName("received_channel_id")]
    public ulong ReceivedChannelId { get; set; } = 0;

    /// <summary>
    /// 送信チャンネルID
    /// </summary>
    [JsonPropertyName("sent_channel_id")]
    public ulong SentChannelId { get; set; } = 0;
}
