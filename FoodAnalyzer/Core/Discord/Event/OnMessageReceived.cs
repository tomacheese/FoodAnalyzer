using System.Text.Encodings.Web;
using System.Text.Json;
using Discord;
using Discord.WebSocket;
using FoodAnalyzer.Core.Azure;
using FoodAnalyzer.Core.Azure.Models;
using FoodAnalyzer.Core.Config;
using FoodAnalyzer.Core.Config.Json;

namespace FoodAnalyzer.Core.Discord.Event;

/// <summary>
/// Discord メッセージ受信時のイベントハンドラー。
/// 添付画像がある場合、画像を解析し、結果を指定チャンネルに送信します。
/// </summary>
internal class OnMessageReceived(DiscordSocketClient client) : IBaseEvent
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// メッセージ受信イベントのハンドラーを登録します。
    /// </summary>
    /// <returns>非同期タスク。</returns>
    public Task RegisterAsync()
    {
        client.MessageReceived += HandleAsync;
        return Task.CompletedTask;
    }

    /// <summary>
    /// メッセージ受信時の処理を実行します。
    /// 添付画像がある場合、画像を解析し、結果を送信します。
    /// </summary>
    /// <param name="message">受信したメッセージ。</param>
    /// <returns>非同期タスク。</returns>
    public async Task HandleAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (message.Attachments.Count == 0) return;

        List<MonitorChannelData> monitorChannels = MonitorManager.GetChannels();
        MonitorChannelData? monitorChannel = monitorChannels
            .FirstOrDefault(channel => channel.ReceivedChannelId == message.Channel.Id);
        if (monitorChannel == null) return;

        IChannel sentChannel = await client.GetChannelAsync(monitorChannel.SentChannelId).ConfigureAwait(false);
        if (sentChannel == null) return;
        if (sentChannel is not SocketTextChannel) return;
        var sentTextChannel = (SocketTextChannel)sentChannel;

        await message.AddReactionAsync(new Emoji("👀")).ConfigureAwait(false);

        ConfigData config = AppConfig.Instance;
        AzureOpenAI openAI = new(config.Azure.Endpoint, config.Azure.ApiKey, config.Azure.Deployment);
        var attachmentNumber = 0;
        foreach (Attachment attachment in message.Attachments)
        {
            attachmentNumber++;
            var width = attachment.Width;
            var height = attachment.Height;
            if (attachment.ContentType == null || !attachment.ContentType.StartsWith("image/", StringComparison.Ordinal) || width == null || height == null)
            {
                continue;
            }

            FoodAnalysisResponse response = await openAI.AnalyzeFoodAsync(attachment.Url, width.Value, height.Value).ConfigureAwait(false);
            var jsonResponse = JsonSerializer.Serialize(response, _jsonOptions);
            await sentTextChannel.SendMessageAsync($"{message.GetJumpUrl()}@{attachmentNumber}\n総カロリー: {response.Total.Calories} kcal\n```json\n{jsonResponse}\n```").ConfigureAwait(false);
        }

        await message.AddReactionAsync(new Emoji("✅")).ConfigureAwait(false);
        await message.RemoveReactionAsync(new Emoji("👀"), client.CurrentUser).ConfigureAwait(false);
    }
}
