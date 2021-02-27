using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnReadyHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ReadyEventHandler _readyEventHandler;
        
        public OnReadyHostedService(DiscordSocketClient discordSocketClient, ReadyEventHandler readyEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _readyEventHandler = readyEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Ready += _readyEventHandler.OnReady;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Ready -= _readyEventHandler.OnReady;
            
            return Task.CompletedTask;
        }
    }
}