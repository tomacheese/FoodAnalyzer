using Discord;
using Discord.WebSocket;
using FoodAnalyzer.Core.Config;
using FoodAnalyzer.Core.Config.Json;

namespace FoodAnalyzer.Core.Discord.Event;

internal class OnMessageReceived(DiscordSocketClient client) : IBaseEvent
{
    public Task RegisterAsync()
    {
        client.MessageReceived += HandleAsync;
        return Task.CompletedTask;
    }

    public async Task HandleAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        List<MonitorChannelData> monitorChannels = MonitorManager.GetChannels();
        MonitorChannelData? monitorChannel = monitorChannels
            .FirstOrDefault(channel => channel.ReceivedChannelId == message.Channel.Id);
        if (monitorChannel == null) return;

        IChannel sentChannel = await client.GetChannelAsync(monitorChannel.SentChannelId).ConfigureAwait(false);
        if (sentChannel == null) return;
        if (sentChannel is not SocketTextChannel) return;
        var sentTextChannel = (SocketTextChannel)sentChannel;

        foreach (Attachment attachment in message.Attachments)
        {
            await sentTextChannel.SendMessageAsync(attachment.Url).ConfigureAwait(false);
        }
    }
}
