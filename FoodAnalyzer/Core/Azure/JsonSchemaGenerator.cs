using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Azure;

internal static class JsonSchemaGenerator
{
    public static string GenerateSchema<T>()
    {
        Type type = typeof(T);
        var schema = GenerateSchemaForType(type);
        return JsonSerializer.Serialize(schema);
    }

    private static object GenerateSchemaForType(Type type, PropertyInfo? propertyInfo = null)
    {
        if (type == typeof(string))
        {
            return new { type = "string" };
        }

        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal) ||
            type == typeof(int) || type == typeof(long))
        {
            // Check for range attributes
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

        if (type == typeof(bool))
        {
            return new { type = "boolean" };
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type itemType = type.GetGenericArguments()[0];
            return new
            {
                type = "array",
                items = GenerateSchemaForType(itemType),
            };
        }

        // Complex object
        var properties = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (PropertyInfo prop in type.GetProperties())
        {
            var jsonPropertyName = prop.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                .Cast<JsonPropertyNameAttribute>()
                .FirstOrDefault()?.Name ?? prop.Name;

            properties[jsonPropertyName] = GenerateSchemaForType(prop.PropertyType, prop);

            // All properties are required
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
