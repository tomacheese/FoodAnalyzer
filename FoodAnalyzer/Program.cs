using Discord;
using Discord.WebSocket;
using FoodAnalyzer.Core.Discord;

namespace FoodAnalyzer;

/// <summary>
/// アプリケーションのエントリポイントを提供します。
/// </summary>
internal class Program
{
    /// <summary>
    /// Discord クライアントを初期化し、メインループを開始します。
    /// </summary>
    /// <returns>非同期タスク。</returns>
    public static async Task Main()
    {
        // とりあえず仮置き…
        var token = File.ReadAllText("token.txt").Trim();

        var discordClient = new DiscordClient(token);
        await discordClient.StartAsync().ConfigureAwait(false);

        await Task.Delay(-1).ConfigureAwait(false);
    }

    /// <summary>
    /// メッセージ受信時の処理を行います。
    /// 添付ファイルの URL をコンソールに出力します。
    /// </summary>
    /// <param name="message">受信したメッセージ。</param>
    /// <returns>非同期タスク。</returns>
    private static async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot)
            return;
        if (message.Channel.Id != 840825964027445269)
            return;

        foreach (Attachment attachment in message.Attachments)
        {
            Console.WriteLine($"Attachment URL: {attachment.Url}");
        }
    }
}
