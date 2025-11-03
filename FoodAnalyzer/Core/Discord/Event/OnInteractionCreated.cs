using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace FoodAnalyzer.Core.Discord.Event;

internal class OnInteractionCreated(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider) : IBaseEvent
{
    public Task RegisterAsync()
    {
        client.InteractionCreated += HandleAsync;
        return Task.CompletedTask;
    }

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
