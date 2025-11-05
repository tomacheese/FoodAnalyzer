namespace FoodAnalyzer.Core.Azure;

/// <summary>
/// 属性に適用し、JSONスキーマの数値範囲（最小値・最大値）を指定する属性です。
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class JsonSchemaRangeAttribute(double minimum, double maximum) : Attribute
{
    /// <summary>
    /// 許容される最小値を取得します。
    /// </summary>
    public double Minimum { get; } = minimum;

    /// <summary>
    /// 許容される最大値を取得します。
    /// </summary>
    public double Maximum { get; } = maximum;
}
