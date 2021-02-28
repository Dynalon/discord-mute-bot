using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnLoggedOutHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly LoggedOutEventHandler _loggedOutEventHandler;
        
        public OnLoggedOutHostedService(DiscordSocketClient discordSocketClient, LoggedOutEventHandler loggedOutEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _loggedOutEventHandler = loggedOutEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.LoggedOut += _loggedOutEventHandler.OnLoggedOut;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.LoggedOut -= _loggedOutEventHandler.OnLoggedOut;
            
            return Task.CompletedTask;
        }
    }
}