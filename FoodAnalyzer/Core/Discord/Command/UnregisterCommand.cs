using Discord.Interactions;
using FoodAnalyzer.Core.Config;

namespace FoodAnalyzer.Core.Discord.Command;

/// <summary>
/// Discord Slash Command : unregister
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1515", Justification = "コマンドクラスのため、外部から参照できる必要がある")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079", Justification = "コマンドクラスのため、外部から参照できる必要がある")]
public class UnregisterCommand : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// チャンネルを監視対象から削除する。
    /// </summary>
    /// <returns>非同期タスク</returns>
    [SlashCommand("unregister", "このチャンネルを監視対象から削除する")]
    public async Task RegisterAsync()
    {
        MonitorManager.RemoveChannel(Context.Guild.Id, Context.Channel.Id);

        await RespondAsync($"このチャンネルを監視対象から削除しました。", ephemeral: true).ConfigureAwait(false);
    }
}
