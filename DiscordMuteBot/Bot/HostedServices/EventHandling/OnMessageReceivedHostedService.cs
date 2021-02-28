using System.Threading;
using System.Threading.Tasks;
using Bot.EventHandlers;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;

namespace Bot.HostedServices.EventHandling
{
    public sealed class OnMessageReceivedHostedService : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly MessageReceivedEventHandler _messageReceivedHandler;
        
        public OnMessageReceivedHostedService(DiscordSocketClient discordSocketClient, MessageReceivedEventHandler messageReceivedHandler)
        {
            _discordSocketClient = discordSocketClient;
            _messageReceivedHandler = messageReceivedHandler;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived += _messageReceivedHandler.OnMessageReceived;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived -= _messageReceivedHandler.OnMessageReceived;
            
            return Task.CompletedTask;
        }
    }
}