using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnUserVoiceStateUpdatedHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly UserVoiceStateUpdatedEventHandler _userVoiceStateUpdatedEventHandler;
        
        public OnUserVoiceStateUpdatedHostedService(DiscordSocketClient discordSocketClient, UserVoiceStateUpdatedEventHandler userVoiceStateUpdatedEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _userVoiceStateUpdatedEventHandler = userVoiceStateUpdatedEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.UserVoiceStateUpdated += _userVoiceStateUpdatedEventHandler.OnUserVoiceStateUpdated;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.UserVoiceStateUpdated -= _userVoiceStateUpdatedEventHandler.OnUserVoiceStateUpdated;
            
            return Task.CompletedTask;
        }
    }
}