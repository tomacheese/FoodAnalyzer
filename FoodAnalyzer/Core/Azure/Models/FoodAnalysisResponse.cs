using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Azure.Models;

public class FoodAnalysisResponse
{
    [JsonPropertyName("items")]
    public List<FoodItem> Items { get; set; } = [];

    [JsonPropertyName("total")]
    public FoodTotal Total { get; set; } = new();
}

public class FoodItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("calories")]
    public double Calories { get; set; }

    [JsonPropertyName("protein_g")]
    public double ProteinG { get; set; }

    [JsonPropertyName("fat_g")]
    public double FatG { get; set; }

    [JsonPropertyName("carbs_g")]
    public double CarbsG { get; set; }

    [JsonPropertyName("allergens")]
    public List<string> Allergens { get; set; } = [];

    [JsonPropertyName("cuisine")]
    public string Cuisine { get; set; } = string.Empty;

    [JsonPropertyName("cooking_method")]
    public string CookingMethod { get; set; } = string.Empty;

    [JsonPropertyName("bbox")]
    public BoundingBox BBox { get; set; } = new();
}

public class BoundingBox
{
    [JsonPropertyName("x_ratio")]
    [JsonSchemaRange(0, 1)]
    public double XRatio { get; set; }

    [JsonPropertyName("y_ratio")]
    [JsonSchemaRange(0, 1)]
    public double YRatio { get; set; }

    [JsonPropertyName("width_ratio")]
    [JsonSchemaRange(0, 1)]
    public double WidthRatio { get; set; }

    [JsonPropertyName("height_ratio")]
    [JsonSchemaRange(0, 1)]
    public double HeightRatio { get; set; }
}

public class FoodTotal
{
    [JsonPropertyName("calories")]
    public double Calories { get; set; }

    [JsonPropertyName("protein_g")]
    public double ProteinG { get; set; }

    [JsonPropertyName("fat_g")]
    public double FatG { get; set; }

    [JsonPropertyName("carbs_g")]
    public double CarbsG { get; set; }

    [JsonPropertyName("allergens")]
    public List<string> Allergens { get; set; } = [];

    [JsonPropertyName("cuisine")]
    public List<string> Cuisine { get; set; } = [];

    [JsonPropertyName("cooking_method")]
    public List<string> CookingMethod { get; set; } = [];

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
