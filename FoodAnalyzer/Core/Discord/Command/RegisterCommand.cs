using Discord.Interactions;

namespace FoodAnalyzer.Core.Discord.Command;

/// <summary>
/// Discord Slash Command : register
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515", Justification = "コマンドクラスのため、外部から参照できる必要がある")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079", Justification = "コマンドクラスのため、外部から参照できる必要がある")]
public class RegisterCommand : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// チャンネルを監視対象に登録する。
    /// </summary>
    /// <returns>非同期タスク。</returns>
    [SlashCommand("register", "チャンネルを監視対象に登録する")]
    public async Task RegisterAsync()
    {

    }
}
