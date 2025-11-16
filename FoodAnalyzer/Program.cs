using FoodAnalyzer.Core.Config;
using FoodAnalyzer.Core.Config.Json;
using FoodAnalyzer.Core.Discord;

namespace FoodAnalyzer;

/// <summary>
/// アプリケーションのエントリポイント
/// </summary>
internal class Program
{
    /// <summary>
    /// Discord クライアントを初期化し、メインループを開始する
    /// </summary>
    /// <returns>非同期処理を表すタスク</returns>
    public static async Task Main()
    {
        ConfigData config = AppConfig.Instance;

        using var discordClient = new DiscordClient(config.Discord.Token);
        await discordClient.StartAsync().ConfigureAwait(false);

        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            await Task.Delay(Timeout.Infinite, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
    }
}
