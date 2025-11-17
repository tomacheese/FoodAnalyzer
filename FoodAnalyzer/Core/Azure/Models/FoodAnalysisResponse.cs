using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FoodAnalyzer.Core.Azure.Models;

/// <summary>
/// 食品分析のレスポンス全体
/// </summary>
internal class FoodAnalysisResponse
{
    /// <summary>
    /// 個々の食品アイテムのリスト
    /// </summary>
    [JsonPropertyName("items")]
    public List<FoodItem> Items { get; set; } = [];

    /// <summary>
    /// 合計の栄養情報
    /// </summary>
    [JsonPropertyName("total")]
    public FoodTotal Total { get; set; } = new();
}

/// <summary>
/// 単一の食品アイテムの情報
/// </summary>
[SuppressMessage("CodeQuality", "IDE0079", Justification = "JSON シリアライザーで使用されるため")]
[SuppressMessage("Performance", "CA1812", Justification = "JSON シリアライザーで使用されるため")]
internal class FoodItem
{
    /// <summary>
    /// 食品名
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// カロリー
    /// </summary>
    [JsonPropertyName("calories")]
    public double Calories { get; set; }

    /// <summary>
    /// タンパク質量（g）
    /// </summary>
    [JsonPropertyName("protein_g")]
    public double ProteinG { get; set; }

    /// <summary>
    /// 脂質量（g）
    /// </summary>
    [JsonPropertyName("fat_g")]
    public double FatG { get; set; }

    /// <summary>
    /// 炭水化物量（g）
    /// </summary>
    [JsonPropertyName("carbs_g")]
    public double CarbsG { get; set; }

    /// <summary>
    /// 調理方法
    /// </summary>
    [JsonPropertyName("cooking_method")]
    public string CookingMethod { get; set; } = string.Empty;

    /// <summary>
    /// 原材料のリスト
    /// </summary>
    [JsonPropertyName("ingredients")]
    public List<string> Ingredients { get; set; } = [];

    /// <summary>
    /// 画像内のバウンディングボックス情報
    /// </summary>
    [JsonPropertyName("bbox")]
    public BoundingBox BBox { get; set; } = new();
}

/// <summary>
/// バウンディングボックスの比率情報
/// </summary>
internal class BoundingBox
{
    /// <summary>
    /// X座標の比率（0～1）
    /// </summary>
    [JsonPropertyName("x_ratio")]
    [JsonSchemaRange(0, 1)]
    public double XRatio { get; set; }

    /// <summary>
    /// Y座標の比率（0～1）
    /// </summary>
    [JsonPropertyName("y_ratio")]
    [JsonSchemaRange(0, 1)]
    public double YRatio { get; set; }

    /// <summary>
    /// 幅の比率（0～1）
    /// </summary>
    [JsonPropertyName("width_ratio")]
    [JsonSchemaRange(0, 1)]
    public double WidthRatio { get; set; }

    /// <summary>
    /// 高さの比率（0～1）
    /// </summary>
    [JsonPropertyName("height_ratio")]
    [JsonSchemaRange(0, 1)]
    public double HeightRatio { get; set; }
}

/// <summary>
/// 合計の栄養情報
/// </summary>
internal class FoodTotal
{
    /// <summary>
    /// 合計カロリー
    /// </summary>
    [JsonPropertyName("calories")]
    public double Calories { get; set; }

    /// <summary>
    /// 合計タンパク質量（g）
    /// </summary>
    [JsonPropertyName("protein_g")]
    public double ProteinG { get; set; }

    /// <summary>
    /// 合計脂質量（g）
    /// </summary>
    [JsonPropertyName("fat_g")]
    public double FatG { get; set; }

    /// <summary>
    /// 合計炭水化物量（g）
    /// </summary>
    [JsonPropertyName("carbs_g")]
    public double CarbsG { get; set; }

    /// <summary>
    /// 含まれる調理方法のリスト
    /// </summary>
    [JsonPropertyName("cooking_method")]
    public List<string> CookingMethod { get; set; } = [];

    /// <summary>
    /// 原材料のリスト
    /// </summary>
    [JsonPropertyName("ingredients")]
    public List<string> Ingredients { get; set; } = [];

    /// <summary>
    /// メッセージ
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
