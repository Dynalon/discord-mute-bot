using System;
using System.Threading.Tasks;
using Bot.Options;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.EventHandlers
{
    public sealed class MessageReceivedEventHandler
    {
        private readonly ILogger<MessageReceivedEventHandler> _logger;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IOptionsMonitor<BotCommandOptions> _botCommandOptionsMonitor;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        
        public MessageReceivedEventHandler(
            ILogger<MessageReceivedEventHandler> logger,
            DiscordSocketClient discordSocketClient, 
            IOptionsMonitor<BotCommandOptions> botCommandOptionsMonitor,
            CommandService commandService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _discordSocketClient = discordSocketClient;
            _botCommandOptionsMonitor = botCommandOptionsMonitor;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }
        
        public async Task OnMessageReceived(SocketMessage socketMessage)
        {
            int commandStartPosition = 0;
            
            if (socketMessage is SocketUserMessage socketUserMessage && 
                !socketUserMessage.Author.IsBot &&
                socketUserMessage.HasCharPrefix(_botCommandOptionsMonitor.CurrentValue.Prefix, ref commandStartPosition) &&
                !socketUserMessage.HasMentionPrefix(_discordSocketClient.CurrentUser, ref commandStartPosition))
            {
                SocketCommandContext socketCommandContext = new SocketCommandContext(_discordSocketClient, socketUserMessage);
                IResult result = await _commandService.ExecuteAsync(socketCommandContext, commandStartPosition, _serviceProvider);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning(result.ErrorReason);
                    
                    await socketCommandContext.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}