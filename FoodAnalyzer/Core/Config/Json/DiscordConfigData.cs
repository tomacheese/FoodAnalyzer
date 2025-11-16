using System.Reflection;
using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// Discord の設定データ
/// </summary>
internal class DiscordConfigData
{
    /// <summary>
    /// Discord ボットのトークン
    /// </summary>
    [JsonPropertyName("token")]
    [EnvironmentVariable("DISCORD_TOKEN")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 環境変数から設定を適用する
    /// </summary>
    /// <param name="prefix">環境変数のプレフィックス</param>
    public void ApplyEnvironmentVariables(string prefix) => ApplyToProperties(this, prefix);

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
