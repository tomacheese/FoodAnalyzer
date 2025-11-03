using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;
internal class MonitorChannelData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; } = 0;

    [JsonPropertyName("received_channel_id")]
    public ulong ReceivedChannelId { get; set; } = 0;

    [JsonPropertyName("sent_channel_id")]
    public ulong SentChannelId { get; set; } = 0;
}
