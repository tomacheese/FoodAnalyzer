namespace FoodAnalyzer.Core.Config.Json;

/// <summary>
/// 環境変数名を指定する属性
/// </summary>
/// <remarks>
/// 環境変数名を指定する属性を初期化する
/// </remarks>
/// <param name="name">環境変数名(プレフィックスなし)</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class EnvironmentVariableAttribute(string name) : Attribute
{
    /// <summary>
    /// 環境変数名(プレフィックスなし)
    /// </summary>
    public string Name { get; } = name;
}
