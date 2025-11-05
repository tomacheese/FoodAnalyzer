using System.Diagnostics.CodeAnalysis;
using Discord;
using Discord.Interactions;
using FoodAnalyzer.Core.Config;

namespace FoodAnalyzer.Core.Discord.Command;

/// <summary>
/// Discord Slash Command : register
/// </summary>
[SuppressMessage("Maintainability", "CA1515", Justification = "コマンドクラスのため、外部から参照できる必要がある")]
[SuppressMessage("CodeQuality", "IDE0079", Justification = "コマンドクラスのため、外部から参照できる必要がある")]
public class RegisterCommand : InteractionModuleBase<SocketInteractionContext>
{
    /// <summary>
    /// チャンネルを監視対象に登録する。
    /// </summary>
    /// <param name="inputSentChannel">通知対象に登録するチャンネル。指定しない場合はこのチャンネルを登録します。</param>
    /// <returns>非同期タスク</returns>
    [SlashCommand("register", "このチャンネルを監視対象に登録する")]
    public async Task RegisterAsync(
        [ChannelTypes(ChannelType.Text)]
        [Summary("sent-channel", "通知対象に登録するチャンネル。指定しない場合はこのチャンネルを登録します。")]
        IChannel? inputSentChannel = null
    )
    {
        ITextChannel? sentChannel = inputSentChannel as ITextChannel ?? Context.Channel as ITextChannel;
        if (sentChannel == null)
        {
            await RespondAsync("送信先チャンネルがテキストチャンネルではありません。").ConfigureAwait(false);
            return;
        }

        MonitorManager.AddChannel(Context.Guild.Id, Context.Channel.Id, sentChannel.Id);

        await RespondAsync($"このチャンネルを監視対象に登録しました。\n"
            + $"送信先チャンネル: {sentChannel.Mention}").ConfigureAwait(false);
    }
}
