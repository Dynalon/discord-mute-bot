using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnLogHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly LogEventHandler _logEventHandler;
        
        public OnLogHostedService(DiscordSocketClient discordSocketClient, LogEventHandler logEventHandler)
        {
            _discordSocketClient = discordSocketClient;
            _logEventHandler = logEventHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Log += _logEventHandler.OnLog;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Log -= _logEventHandler.OnLog;
            
            return Task.CompletedTask;
        }
    }
}