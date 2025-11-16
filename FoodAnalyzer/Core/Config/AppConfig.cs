using System.Text.Json;
using FoodAnalyzer.Core.Config.Json;

namespace FoodAnalyzer.Core.Config;

/// <summary>
/// アプリケーションの設定を管理するクラス
/// </summary>
internal class AppConfig
{
    private static readonly string _configFilePath = GetConfigFilePath();
    private static ConfigData _instance = new();
    private static bool _isLoaded = false;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// 設定データのインスタンス
    /// </summary>
    public static ConfigData Instance
    {
        get
        {
            // configファイルが存在している状態で一度読み込まれた場合、2回目以降は再読み込みしない
            if (File.Exists(_configFilePath) && !_isLoaded)
            {
                _instance = Load();
                _isLoaded = true;
            }

            return _instance;
        }
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    public static void Save()
    {
        if (_instance != null)
        {
            Save(_instance);
        }
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    /// <param name="config">保存するコンフィグ情報</param>
    private static void Save(ConfigData config)
    {
        var json = JsonSerializer.Serialize(config, _jsonSerializerOptions);
        var directoryPath = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(_configFilePath, json);
    }

    /// <summary>
    /// 設定を読み込む
    /// </summary>
    private static ConfigData Load()
    {
        if (!File.Exists(_configFilePath))
        {
            var defaultConfig = new ConfigData();
            Save(defaultConfig);
            return defaultConfig;
        }

        var json = File.ReadAllText(_configFilePath);
        ConfigData config = JsonSerializer.Deserialize<ConfigData>(json)
                            ?? new ConfigData();

        // 環境変数から設定を上書き
        config.ApplyEnvironmentVariables();

        return config;
    }

    private static string GetConfigFilePath()
    {
        var envPath = Environment.GetEnvironmentVariable("FOODANALYZER_CONFIG_PATH");
        return !string.IsNullOrEmpty(envPath) ? envPath : "config.json";
    }
}
