using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnConnectedHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ConnectedEventHandler _connectedEventHandler;
        
        public OnConnectedHostedService(DiscordSocketClient discordSocketClient, ConnectedEventHandler connectedEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _connectedEventHandler = connectedEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Connected += _connectedEventHandler.OnConnected;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Connected -= _connectedEventHandler.OnConnected;
            
            return Task.CompletedTask;
        }
    }
}