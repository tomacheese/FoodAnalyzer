using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Azure;

/// <summary>
/// 型情報からJSON Schemaを生成するユーティリティクラス
/// </summary>
internal static class JsonSchemaGenerator
{
    /// <summary>
    /// 指定された型のJSON Schemaを生成します
    /// </summary>
    /// <typeparam name="T">スキーマを生成する対象の型</typeparam>
    /// <returns>JSON形式のスキーマ文字列</returns>
    public static string GenerateSchema<T>()
    {
        Type type = typeof(T);
        var schema = GenerateSchemaForType(type);
        return JsonSerializer.Serialize(schema);
    }

    /// <summary>
    /// 型情報に基づいてスキーマオブジェクトを生成します
    /// </summary>
    /// <param name="type">スキーマを生成する対象の型</param>
    /// <param name="propertyInfo">プロパティ情報（属性の取得に使用）</param>
    /// <returns>スキーマを表すオブジェクト</returns>
    private static object GenerateSchemaForType(Type type, PropertyInfo? propertyInfo = null)
    {
        // 文字列型の処理
        if (type == typeof(string))
        {
            return new { type = "string" };
        }

        // 数値型の処理（範囲制約を含む）
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal) ||
            type == typeof(int) || type == typeof(long))
        {
            // 範囲属性のチェック
            if (propertyInfo != null)
            {
                JsonSchemaRangeAttribute? rangeAttr = propertyInfo.GetCustomAttributes(typeof(JsonSchemaRangeAttribute), false)
                    .Cast<JsonSchemaRangeAttribute>()
                    .FirstOrDefault();

                if (rangeAttr != null)
                {
                    return new
                    {
                        type = "number",
                        minimum = rangeAttr.Minimum,
                        maximum = rangeAttr.Maximum,
                    };
                }
            }

            return new { type = "number" };
        }

        // 真偽値型の処理
        if (type == typeof(bool))
        {
            return new { type = "boolean" };
        }

        // リスト型（配列）の処理
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type itemType = type.GetGenericArguments()[0];
            return new
            {
                type = "array",
                items = GenerateSchemaForType(itemType),
            };
        }

        // 複雑なオブジェクト型の処理
        var properties = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (PropertyInfo prop in type.GetProperties())
        {
            // JsonPropertyName属性からプロパティ名を取得（指定がない場合はプロパティ名を使用）
            var jsonPropertyName = prop.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                .Cast<JsonPropertyNameAttribute>()
                .FirstOrDefault()?.Name ?? prop.Name;

            properties[jsonPropertyName] = GenerateSchemaForType(prop.PropertyType, prop);

            // すべてのプロパティを必須として扱う
            required.Add(jsonPropertyName);
        }

        return new
        {
            type = "object",
            properties,
            required = required.ToArray(),
            additionalProperties = false,
        };
    }
}
