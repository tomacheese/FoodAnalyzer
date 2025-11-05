using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FoodAnalyzer.Core.Discord.Event;

/// <summary>
/// Discordのインタラクションイベント（スラッシュコマンド等）を処理するイベントハンドラ。
/// </summary>
internal class OnInteractionCreated(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider) : IBaseEvent
{
    /// <summary>
    /// イベントハンドラをDiscordクライアントに登録します。
    /// </summary>
    /// <returns>非同期タスク。</returns>
    public Task RegisterAsync()
    {
        client.InteractionCreated += HandleAsync;
        return Task.CompletedTask;
    }

    /// <summary>
    /// インタラクションが発生した際に呼び出され、コマンドの実行を試みます。
    /// </summary>
    /// <param name="socketInteraction">発生したインタラクション。</param>
    /// <returns>非同期タスク。</returns>
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
