using System.Reflection;
using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// アプリケーションの設定データ
/// </summary>
internal class ConfigData
{
    private const string EnvPrefix = "FOODANALYZER_";

    /// <summary>
    /// 設定ファイルのバージョン
    /// </summary>
    [JsonPropertyName("version")]
    [EnvironmentVariable("VERSION")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// Discord 関連の設定データ
    /// </summary>
    [JsonPropertyName("discord")]
    public DiscordConfigData Discord { get; set; } = new();

    /// <summary>
    /// Azure 関連の設定データ
    /// </summary>
    [JsonPropertyName("azure")]
    public AzureConfigData Azure { get; set; } = new();

    /// <summary>
    /// 環境変数から設定を適用する
    /// </summary>
    public void ApplyEnvironmentVariables()
    {
        // 自身のプロパティに環境変数を適用
        ApplyToProperties(this, EnvPrefix);

        // 子設定クラスにも適用
        Discord.ApplyEnvironmentVariables(EnvPrefix);
        Azure.ApplyEnvironmentVariables(EnvPrefix);
    }

    /// <summary>
    /// オブジェクトのプロパティに環境変数を適用する
    /// </summary>
    /// <param name="obj">対象オブジェクト</param>
    /// <param name="prefix">環境変数のプレフィックス</param>
    private static void ApplyToProperties(object obj, string prefix)
    {
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            EnvironmentVariableAttribute? attribute = property.GetCustomAttribute<EnvironmentVariableAttribute>();
            if (attribute == null)
            {
                continue;
            }

            var envVarName = $"{prefix}{attribute.Name}";
            var envValue = Environment.GetEnvironmentVariable(envVarName);

            if (string.IsNullOrEmpty(envValue))
            {
                continue;
            }

            // 型に応じて値を変換して設定
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(obj, envValue);
            }
            else if (property.PropertyType == typeof(int) && int.TryParse(envValue, out var intValue))
            {
                property.SetValue(obj, intValue);
            }
        }
    }
}
