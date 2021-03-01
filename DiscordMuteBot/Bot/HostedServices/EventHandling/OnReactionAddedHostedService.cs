using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnReactionAddedHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ReactionAddedEventHandler _reactionAddedEventHandler;

        public OnReactionAddedHostedService(DiscordSocketClient discordSocketClient, ReactionAddedEventHandler reactionAddedEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _reactionAddedEventHandler = reactionAddedEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.ReactionAdded += _reactionAddedEventHandler.OnReactionAdded;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.ReactionAdded -= _reactionAddedEventHandler.OnReactionAdded;
            
            return Task.CompletedTask;
        }
    }
}