using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnDisconnectedHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly DisconnectedEventHandler _disconnectedEventHandler;
        
        public OnDisconnectedHostedService(DiscordSocketClient discordSocketClient, DisconnectedEventHandler disconnectedEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _disconnectedEventHandler = disconnectedEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Disconnected += _disconnectedEventHandler.OnDisconnected;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Disconnected -= _disconnectedEventHandler.OnDisconnected;
            
            return Task.CompletedTask;
        }
    }
}