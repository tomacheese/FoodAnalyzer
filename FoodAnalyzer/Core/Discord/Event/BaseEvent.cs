namespace FoodAnalyzer.Core.Discord.Event;

/// <summary>
/// ベースとなる Discord イベントインターフェース
/// </summary>
internal interface IBaseEvent
{
    /// <summary>
    /// イベントの登録処理を行う。
    /// </summary>
    /// <returns>完了したTask</returns>
    Task RegisterAsync() => Task.CompletedTask;
}
