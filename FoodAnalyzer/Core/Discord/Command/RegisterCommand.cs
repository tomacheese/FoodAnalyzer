using Discord;
using Discord.Interactions;
using FoodAnalyzer.Core.Config;

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
    /// <returns>非同期タスク</returns>
    [SlashCommand("register", "このチャンネルを監視対象に登録する")]
    public async Task RegisterAsync(
        [ChannelTypes(ChannelType.Text)]
        [Summary("sent-channel", "通知対象に登録するチャンネル。指定しない場合はこのチャンネルを登録します。")]
        IChannel inputSentChannel = null!
    )
    {
        ITextChannel sentChannel = inputSentChannel != null ? inputSentChannel as ITextChannel : Context.Channel as ITextChannel;
        MonitorManager.AddChannel(Context.Guild.Id, Context.Channel.Id, sentChannel?.Id ?? Context.Channel.Id);

        await RespondAsync($"このチャンネルを監視対象に登録しました。\n"
            + $"送信先チャンネル: {sentChannel.Mention}", ephemeral: true).ConfigureAwait(false);
    }
}
