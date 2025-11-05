using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FoodAnalyzer.Core.Discord.Event;
using Microsoft.Extensions.DependencyInjection;

namespace FoodAnalyzer.Core.Discord;

/// <summary>
/// Discord クライアントのラッパークラス。Bot の初期化とイベント登録を行う
/// </summary>
internal class DiscordClient : IDisposable
{
    private readonly string _token;

    /// <summary>
    /// 内部の <see cref="DiscordSocketClient"/> インスタンス
    /// </summary>
    private readonly DiscordSocketClient _client;

    /// <summary>
    /// インタラクションサービス
    /// </summary>
    private readonly InteractionService _interactionService;

    /// <summary>
    /// 依存性注入サービスプロバイダー
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 指定したトークンで Discord クライアントを初期化する
    /// </summary>
    /// <param name="token">Bot の認証トークン</param>
    public DiscordClient(string token)
    {
        _token = token;

        var config = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
        };
        var client = new DiscordSocketClient(config);

        _client = client;
        _interactionService = new InteractionService(client);
        _serviceProvider = BuildServiceProvider();

        RegisterEventsAsync(client);
    }

    /// <summary>
    /// 依存性注入サービスプロバイダーを構築する
    /// </summary>
    /// <returns><see cref="ServiceProvider"/> インスタンス</returns>
    private ServiceProvider BuildServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_interactionService)
            .BuildServiceProvider();
    }

    /// <summary>
    /// 必要なイベントハンドラをクライアントに登録する
    /// </summary>
    /// <param name="client">イベント登録対象の <see cref="DiscordSocketClient"/></param>
    private void RegisterEventsAsync(DiscordSocketClient client)
    {
        var events = new IBaseEvent[]
        {
            new DiscordLogger(client),
            new OnReady(client, _interactionService, _serviceProvider),
            new OnInteractionCreated(client, _interactionService, _serviceProvider),
            new OnMessageReceived(client),
        };

        foreach (IBaseEvent e in events)
        {
            Console.WriteLine($"Register: {e}");
            e.RegisterAsync().GetAwaiter().GetResult();
        }

        Console.WriteLine("Discord events registered.");
    }

    /// <summary>
    /// Discord クライアントを起動し、Bot としてログインする
    /// </summary>
    /// <returns>非同期処理を表すタスク</returns>
    public async Task StartAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _token).ConfigureAwait(false);
        await _client.StartAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// リソースを解放する
    /// </summary>
    public void Dispose()
    {
        _interactionService.Dispose();
        _client.Dispose();
    }
}
