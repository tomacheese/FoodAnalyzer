using System.Text.Json;
using Discord;
using Discord.WebSocket;
using FoodAnalyzer.Core.Azure;
using FoodAnalyzer.Core.Azure.Models;
using FoodAnalyzer.Core.Config;
using FoodAnalyzer.Core.Config.Json;

namespace FoodAnalyzer.Core.Discord.Event;

internal class OnMessageReceived(DiscordSocketClient client) : IBaseEvent
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public Task RegisterAsync()
    {
        client.MessageReceived += HandleAsync;
        return Task.CompletedTask;
    }

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
            var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);
            await sentTextChannel.SendMessageAsync($"{message.GetJumpUrl()}@{attachmentNumber}\n総カロリー: {response.Total.Calories} kcal\n```json\n{jsonResponse}\n```").ConfigureAwait(false);
        }

        await message.AddReactionAsync(new Emoji("✅")).ConfigureAwait(false);
        await message.RemoveReactionAsync(new Emoji("👀"), client.CurrentUser).ConfigureAwait(false);
    }
}
