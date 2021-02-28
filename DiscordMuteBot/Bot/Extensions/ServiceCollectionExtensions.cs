using System;
using Bot.EventHandlers;
using Bot.HostedServices.EventHandling;
using Bot.Options;
using Bot.OptionsValidation;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Bot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IServiceProvider>(serviceProvider => serviceProvider)
                .ConfigureOptionValidators()
                .ConfigureOptions()
                .ConfigureDiscordSocketClient()
                .ConfigureBotCommands()
                .ConfigureBotEventHandlers()
                .ConfigureHostedServices();
        
        private static IServiceCollection ConfigureOptionValidators(this IServiceCollection serviceCollection)
            => serviceCollection.AddSingleton<IValidateOptions<BotOptions>, BotOptionsValidator>();

        private static IServiceCollection ConfigureOptions(this IServiceCollection serviceCollection)
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            
            IConfigurationSection botConfigurationSection = configuration.GetSection("Bot");
            serviceCollection.Configure<BotOptions>(botConfigurationSection);
            
            IConfigurationSection botCommandsConfigurationSection = configuration.GetSection("BotCommands");
            serviceCollection.Configure<BotCommandOptions>(botCommandsConfigurationSection);
            
            return serviceCollection;
        }

        private static IServiceCollection ConfigureDiscordSocketClient(this IServiceCollection serviceCollection) 
            => serviceCollection.AddSingleton<DiscordSocketClient>(serviceProvider =>
            {
                IOptionsMonitor<BotOptions> botOptionsMonitor = serviceProvider.GetService<IOptionsMonitor<BotOptions>>();
                BotOptions botOptions = botOptionsMonitor.CurrentValue;
                
                DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig()
                {
                    LogLevel = botOptions.LogSeverity
                };
                
                return new DiscordSocketClient(discordSocketConfig);
            });
        
        private static IServiceCollection ConfigureBotCommands(this IServiceCollection serviceCollection) 
            => serviceCollection.AddSingleton<CommandService>(serviceProvider =>
            {
                IOptionsMonitor<BotCommandOptions> botCommandOptionsMonitor = serviceProvider.GetService<IOptionsMonitor<BotCommandOptions>>();
                BotCommandOptions botCommandOptions = botCommandOptionsMonitor.CurrentValue;
                
                CommandServiceConfig commandServiceConfig = new CommandServiceConfig()
                {
                    LogLevel = botCommandOptions.LogSeverity,
                    DefaultRunMode = botCommandOptions.DefaultRunMode,
                    CaseSensitiveCommands = botCommandOptions.CaseSensitiveCommands
                };
                
                return new CommandService(commandServiceConfig);
            });
        
        private static IServiceCollection ConfigureBotEventHandlers(this IServiceCollection serviceCollection) 
            => serviceCollection
                .AddSingleton<ReadyEventHandler>()
                .AddSingleton<ConnectedEventHandler>()
                .AddSingleton<DisconnectedEventHandler>();
        
        private static IServiceCollection ConfigureHostedServices(this IServiceCollection serviceCollection) 
            => serviceCollection
                .AddHostedService<OnReadyHostedService>()
                .AddHostedService<OnConnectedHostedService>()
                .AddHostedService<OnDisconnectedHostedService>();
    }
}