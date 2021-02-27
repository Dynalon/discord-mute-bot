using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bot.Options;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot
{
    public sealed class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IOptionsMonitor<BotOptions> _botOptionsMonitor;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        
        public Worker(
            ILogger<Worker> logger, 
            DiscordSocketClient discordSocketClient,
            IOptionsMonitor<BotOptions> botOptionsMonitor,
            CommandService commandService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _discordSocketClient = discordSocketClient;
            _botOptionsMonitor = botOptionsMonitor;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting service");
            
            await _discordSocketClient.LoginAsync(TokenType.Bot, _botOptionsMonitor.CurrentValue.Token);
            await _discordSocketClient.StartAsync();
            
            Assembly commandsAssembly = Assembly.GetEntryAssembly();
            await _commandService.AddModulesAsync(commandsAssembly, _serviceProvider);
            
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping service");

            await _discordSocketClient.LogoutAsync();
            await _discordSocketClient.StopAsync();
            
            await base.StopAsync(cancellationToken);
        }
    }
}