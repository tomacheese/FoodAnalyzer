using System.Text.Json;
using FoodAnalyzer.Core.Config.Json;

namespace FoodAnalyzer.Core.Config;

/// <summary>
/// 管理対象のチャンネル情報を管理するクラス
/// モニター設定の読み込み・保存・チャンネル情報の追加/削除を行う
/// </summary>
internal class MonitorManager
{
    private static readonly string _monitorFilePath = GetMonitorFilePath();
    private static MonitorData _instance = new();
    private static bool _isLoaded = false;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// モニターデータのインスタンス
    /// </summary>
    public static MonitorData Instance
    {
        get
        {
            if (File.Exists(_monitorFilePath) && !_isLoaded)
            {
                _instance = Load();
                _isLoaded = true;
            }

            return _instance;
        }
    }

    /// <summary>
    /// 管理対象のチャンネル情報リストを取得する
    /// </summary>
    /// <returns>チャンネル情報リスト</returns>
    public static List<MonitorChannelData> GetChannels() => Instance.Channels;

    /// <summary>
    /// 管理対象のチャンネル情報を追加する
    /// 既に同じギルドIDと受信チャンネルIDの組み合わせが存在する場合は、既存のエントリを削除してから追加する
    /// </summary>
    /// <param name="guildId">ギルドID</param>
    /// <param name="receivedChannelId">受信チャンネルID</param>
    /// <param name="sentChannelId">送信チャンネルID</param>
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

    /// <summary>
    /// 指定したギルドIDと受信チャンネルIDに一致するチャンネル情報を削除する
    /// </summary>
    /// <param name="guildId">ギルドID</param>
    /// <param name="receivedChannelId">受信チャンネルID</param>
    public static void RemoveChannel(ulong guildId, ulong receivedChannelId)
    {
        MonitorData monitor = Instance;
        monitor.Channels.RemoveAll(c => c.GuildId == guildId && c.ReceivedChannelId == receivedChannelId);
        Save(monitor);
    }

    /// <summary>
    /// 現在のモニターデータを保存する
    /// </summary>
    public static void Save()
    {
        if (_instance != null)
        {
            Save(_instance);
        }
    }

    /// <summary>
    /// 指定したモニターデータをファイルに保存する
    /// </summary>
    /// <param name="monitor">保存するモニターデータ</param>
    private static void Save(MonitorData monitor)
    {
        var json = JsonSerializer.Serialize(monitor, _jsonSerializerOptions);
        var directoryPath = Path.GetDirectoryName(_monitorFilePath);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        File.WriteAllText(_monitorFilePath, json);
    }

    /// <summary>
    /// モニターデータをファイルから読み込む
    /// </summary>
    /// <returns>モニターデータ</returns>
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

    /// <summary>
    /// モニターファイルのパスを取得する
    /// </summary>
    /// <returns>モニターファイルパス</returns>
    private static string GetMonitorFilePath()
    {
        var envPath = Environment.GetEnvironmentVariable("FOODANALYZER_MONITOR_PATH");
        return !string.IsNullOrEmpty(envPath) ? envPath : "monitor.json";
    }
}
