namespace FoodAnalyzer.Core.Azure;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class JsonSchemaRangeAttribute(double minimum, double maximum) : Attribute
{
    public double Minimum { get; } = minimum;

    public double Maximum { get; } = maximum;
}
