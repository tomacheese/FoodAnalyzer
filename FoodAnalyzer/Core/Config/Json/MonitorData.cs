using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;
internal class MonitorData
{
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    [JsonPropertyName("channels")]
    public List<MonitorChannelData> Channels { get; set; } = [];
}
