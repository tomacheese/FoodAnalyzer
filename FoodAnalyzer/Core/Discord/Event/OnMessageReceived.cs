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
/// Discord メッセージ受信時のイベントハンドラー
/// 添付画像がある場合、画像を解析し、結果を指定チャンネルに送信する
/// </summary>
internal class OnMessageReceived(DiscordSocketClient client) : IBaseEvent
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// メッセージ受信イベントのハンドラーを登録する
    /// </summary>
    /// <returns>完了を表すタスク</returns>
    public Task RegisterAsync()
    {
        client.MessageReceived += HandleAsync;
        return Task.CompletedTask;
    }

    /// <summary>
    /// メッセージ受信時の処理を実行する
    /// 添付画像がある場合、画像を解析し、結果を送信する
    /// </summary>
    /// <param name="message">受信したメッセージ</param>
    /// <returns>非同期処理を表すタスク</returns>
    public async Task HandleAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (message.Attachments.Count == 0) return;

        MonitorChannelData? monitorChannel = GetMonitorChannel(message.Channel.Id);
        if (monitorChannel == null) return;

        SocketTextChannel? sentTextChannel = await GetSentTextChannelAsync(monitorChannel.SentChannelId).ConfigureAwait(false);
        if (sentTextChannel == null) return;

        await message.AddReactionAsync(new Emoji("👀")).ConfigureAwait(false);

        try
        {
            ConfigData config = AppConfig.Instance;
            var openAI = new AzureOpenAI(config.Azure.Endpoint, config.Azure.ApiKey, config.Azure.Deployment);

            await AnalyzeAndSendAttachmentsAsync(message, sentTextChannel, openAI).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await SendLongMessageAsync(sentTextChannel, $"{message.GetJumpUrl()}\nエラーが発生しました: {ex.Message}").ConfigureAwait(false);
            await message.AddReactionAsync(new Emoji("❌")).ConfigureAwait(false);
            await message.RemoveReactionAsync(new Emoji("👀"), client.CurrentUser).ConfigureAwait(false);
            return;
        }

        await message.AddReactionAsync(new Emoji("✅")).ConfigureAwait(false);
        await message.RemoveReactionAsync(new Emoji("👀"), client.CurrentUser).ConfigureAwait(false);
    }

    private static MonitorChannelData? GetMonitorChannel(ulong channelId)
    {
        List<MonitorChannelData> monitorChannels = MonitorManager.GetChannels();
        return monitorChannels.FirstOrDefault(channel => channel.ReceivedChannelId == channelId);
    }

    private async Task<SocketTextChannel?> GetSentTextChannelAsync(ulong sentChannelId)
    {
        IChannel sentChannel = await client.GetChannelAsync(sentChannelId).ConfigureAwait(false);
        return sentChannel as SocketTextChannel;
    }

    private static async Task AnalyzeAndSendAttachmentsAsync(SocketMessage message, SocketTextChannel sentTextChannel, AzureOpenAI openAI)
    {
        var attachmentNumber = 0;
        foreach (Attachment attachment in message.Attachments)
        {
            attachmentNumber++;
            if (!IsImageAttachment(attachment)) continue;

            FoodAnalysisResponse response = await openAI.AnalyzeFoodAsync(attachment.Url, attachment.Width!.Value, attachment.Height!.Value).ConfigureAwait(false);
            var jsonResponse = JsonSerializer.Serialize(response, _jsonOptions);
            var messageContent = $"{message.GetJumpUrl()}@{attachmentNumber}\n総カロリー: {response.Total.Calories} kcal\n```json\n{jsonResponse}\n```";

            await SendLongMessageAsync(sentTextChannel, messageContent).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 長いメッセージを改行単位で分割して送信する
    /// Discordの文字数制限（2000文字）を超える場合に対応
    /// </summary>
    /// <param name="channel">送信先チャンネル</param>
    /// <param name="message">送信するメッセージ</param>
    /// <returns>非同期処理を表すタスク</returns>
    private static async Task SendLongMessageAsync(SocketTextChannel channel, string message)
    {
        const int maxLength = 2000;

        if (message.Length <= maxLength)
        {
            await channel.SendMessageAsync(message).ConfigureAwait(false);
            return;
        }

        var lines = message.Split('\n');
        var currentMessage = string.Empty;
        var isInCodeBlock = false;
        var codeBlockLanguage = string.Empty;

        foreach (var line in lines)
        {
            var lineWithNewline = currentMessage.Length == 0 ? line : $"\n{line}";

            // コードブロックの開始/終了を検出
            if (line.StartsWith("```", StringComparison.Ordinal))
            {
                if (!isInCodeBlock)
                {
                    isInCodeBlock = true;
                    codeBlockLanguage = line.Length > 3 ? line[3..] : string.Empty;
                }
                else
                {
                    isInCodeBlock = false;
                }
            }

            // 追加すると制限を超える場合
            if (currentMessage.Length + lineWithNewline.Length > maxLength)
            {
                // コードブロック内の場合は閉じる
                if (isInCodeBlock)
                {
                    currentMessage += "\n```";
                }

                await channel.SendMessageAsync(currentMessage).ConfigureAwait(false);

                // 次のメッセージの開始
                currentMessage = isInCodeBlock ? $"```{codeBlockLanguage}\n{line}" : line;
            }
            else
            {
                currentMessage += lineWithNewline;
            }
        }

        // 残りのメッセージを送信
        if (currentMessage.Length > 0)
        {
            // コードブロックが閉じられていない場合は閉じる
            if (isInCodeBlock && !currentMessage.EndsWith("```", StringComparison.Ordinal))
            {
                currentMessage += "\n```";
            }

            await channel.SendMessageAsync(currentMessage).ConfigureAwait(false);
        }
    }

    private static bool IsImageAttachment(Attachment attachment)
    {
        return attachment.ContentType != null
            && attachment.ContentType.StartsWith("image/", StringComparison.Ordinal)
            && attachment.Width.HasValue
            && attachment.Height.HasValue;
    }
}
