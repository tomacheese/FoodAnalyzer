using System.Text.Json;
using FoodAnalyzer.Core.Config.Json;

namespace FoodAnalyzer.Core.Config;
internal class MonitorManager
{
    private static readonly string _monitorFilePath = GetMonitorFilePath();
    private static MonitorData _instance = new();
    private static bool _isLoaded = false;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public static MonitorData Instance
    {
        get
        {
            if (File.Exists(_monitorFilePath) && !_isLoaded)
            {
                _instance = Load();
                _isLoaded = true;
            }

            return _instance!;
        }
    }

    public static List<MonitorChannelData> GetChannels() => Instance.Channels;

    public static void AddChannel(ulong guildId, ulong receivedChannelId, ulong sentChannelId)
    {
        MonitorData monitor = Instance;
        if (monitor.Channels.Any(c => c.GuildId == guildId && c.ReceivedChannelId == receivedChannelId))
        {
            RemoveChannel(guildId, receivedChannelId);
        }

        monitor.Channels.Add(new MonitorChannelData
        {
            GuildId = guildId,
            ReceivedChannelId = receivedChannelId,
            SentChannelId = sentChannelId,
        });
        Save(monitor);
    }

    public static void RemoveChannel(ulong guildId, ulong receivedChannelId)
    {
        MonitorData monitor = Instance;
        monitor.Channels.RemoveAll(c => c.GuildId == guildId && c.ReceivedChannelId == receivedChannelId);
        Save(monitor);
    }

    public static void Save()
    {
        if (_instance != null)
        {
            Save(_instance);
        }
    }

    private static void Save(MonitorData monitor)
    {
        var json = JsonSerializer.Serialize(monitor, _jsonSerializerOptions);
        Directory.CreateDirectory(Directory.GetParent(_monitorFilePath)!.FullName);
        File.WriteAllText(_monitorFilePath, json);
    }

    private static MonitorData Load()
    {
        if (!File.Exists(_monitorFilePath))
        {
            return new MonitorData();
        }

        var json = File.ReadAllText(_monitorFilePath);
        MonitorData? monitor = JsonSerializer.Deserialize<MonitorData>(json);
        return monitor ?? new MonitorData();
    }

    private static string GetMonitorFilePath()
    {
        var envPath = Environment.GetEnvironmentVariable("FOODANALYZER_CONFIG_PATH");
        return !string.IsNullOrEmpty(envPath) ? envPath : "monitor.json";
    }
}
