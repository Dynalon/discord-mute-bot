using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnLoggedInHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly LoggedInEventHandler _loggedInEventHandler;
        
        public OnLoggedInHostedService(DiscordSocketClient discordSocketClient, LoggedInEventHandler loggedInEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _loggedInEventHandler = loggedInEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.LoggedIn += _loggedInEventHandler.OnLoggedIn;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.LoggedIn -= _loggedInEventHandler.OnLoggedIn;
            
            return Task.CompletedTask;
        }
    }
}