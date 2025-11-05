using FoodAnalyzer.Core.Config;
using FoodAnalyzer.Core.Config.Json;
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
        ConfigData config = AppConfig.Instance;

        var discordClient = new DiscordClient(config.Discord.Token);
        await discordClient.StartAsync().ConfigureAwait(false);

        await Task.Delay(-1).ConfigureAwait(false);
    }
}
