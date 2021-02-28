using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnChannelDestroyedHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ChannelDestroyedEventHandler _channelDestroyedEventHandler;
        
        public OnChannelDestroyedHostedService(DiscordSocketClient discordSocketClient, ChannelDestroyedEventHandler channelDestroyedEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _channelDestroyedEventHandler = channelDestroyedEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.ChannelDestroyed += _channelDestroyedEventHandler.OnChannelDestroyed;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.ChannelDestroyed -= _channelDestroyedEventHandler.OnChannelDestroyed;
            
            return Task.CompletedTask;
        }
    }
}