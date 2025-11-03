using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FoodAnalyzer.Core.Discord.Event;

namespace FoodAnalyzer.Core.Discord;

/// <summary>
/// Discordのログイベントを処理するロガークラス。
/// </summary>
/// <remarks>
/// <see cref="DiscordLogger"/> の新しいインスタンスを初期化します。
/// </remarks>
/// <param name="client">Discordクライアントインスタンス。</param>
internal class DiscordLogger(DiscordSocketClient client) : IBaseEvent
{
    /// <summary>
    /// 準備完了イベントのハンドラーを登録する。
    /// </summary>
    /// <returns>完了したTask</returns>
    public Task RegisterAsync()
    {
        client.Log += Handle;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Discordのログイベントを処理します。
    /// </summary>
    /// <param name="message">Discordのログメッセージ。</param>
    /// <returns>完了したTask。</returns>
    public Task Handle(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else
        {
            Console.WriteLine($"[General/{message.Severity}] {message}");
        }

        return Task.CompletedTask;
    }
}
