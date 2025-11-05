using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace FoodAnalyzer.Core.Discord.Event;

/// <summary>
/// Discordクライアントの準備完了時に発生するイベントを処理する
/// </summary>
internal class OnReady(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider) : IBaseEvent
{
    /// <summary>
    /// 準備完了イベントのハンドラーを登録する
    /// </summary>
    /// <returns>完了を表すタスク</returns>
    public Task RegisterAsync()
    {
        client.Ready += HandleAsync;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 準備完了イベントのハンドラー
    /// </summary>
    /// <returns>非同期処理を表すタスク</returns>
    public async Task HandleAsync()
    {
        Console.WriteLine($"Connected as {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");

        await RegisterCommandsAsync().ConfigureAwait(false);
    }

    private async Task RegisterCommandsAsync()
    {
        await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), serviceProvider).ConfigureAwait(false);
        Console.WriteLine($"Modules added to InteractionService. Module count: {interactionService.Modules.Count}");

        // 登録されたコマンドを表示
        foreach (ModuleInfo module in interactionService.Modules)
        {
            Console.WriteLine($"Module: {module.Name}");
            foreach (SlashCommandInfo command in module.SlashCommands)
            {
                Console.WriteLine($"  Command: /{command.Name} - {command.Description}");
            }
        }

        // 次にコマンドをギルドに登録
        foreach (SocketGuild guild in client.Guilds)
        {
            Console.WriteLine($"Registering slash commands to guild: {guild.Name} ({guild.Id})");
            await interactionService.RegisterCommandsToGuildAsync(guild.Id).ConfigureAwait(false);
        }

        Console.WriteLine("Slash commands registered to all guilds.");
    }
}
