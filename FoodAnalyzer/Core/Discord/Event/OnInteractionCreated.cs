using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FoodAnalyzer.Core.Discord.Event;

/// <summary>
/// Discordクライアントのインタラクション作成時に発生するイベントを処理する。
/// </summary>
/// <param name="client">Discordクライアントインスタンス</param>
/// <param name="interactionService">インタラクションサービス</param>
/// <param name="serviceProvider">サービスプロバイダー</param>
internal class OnInteractionCreated(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider) : IBaseEvent
{
    /// <summary>
    /// 準備完了イベントのハンドラーを登録する。
    /// </summary>
    /// <returns>完了したTask</returns>
    public Task RegisterAsync()
    {
        client.InteractionCreated += HandleAsync;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 準備完了イベントのハンドラー。
    /// </summary>
    /// <param name="socketInteraction">受信したDiscordインタラクション</param>
    /// <returns>完了したTask</returns>
    public async Task HandleAsync(SocketInteraction socketInteraction)
    {
        try
        {
            var ctx = new SocketInteractionContext(client, socketInteraction);
            await interactionService.ExecuteCommandAsync(ctx, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Interaction handling failed: {ex}");
            Console.WriteLine(ex.ToString());

            if (socketInteraction.Type == InteractionType.ApplicationCommand && !socketInteraction.HasResponded)
            {
                await socketInteraction.RespondAsync("エラーが発生しました。", ephemeral: true).ConfigureAwait(false);
            }
        }
    }
}
