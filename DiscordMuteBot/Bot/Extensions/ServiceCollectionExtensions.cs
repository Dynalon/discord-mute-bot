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
                .AddSingleton<ObservedVoiceChannelsCache>()
                .ConfigureOptionValidators()
                .ConfigureOptions()
                .ConfigureDiscordSocketClient()
                .ConfigureBotEventHandlers()
                .ConfigureBotCommands()
                .ConfigureHostedServices();
        
        private static IServiceCollection ConfigureOptionValidators(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IValidateOptions<BotOptions>, BotOptionsValidator>()
                .AddSingleton<IValidateOptions<ObservedVoiceChannelOptions>, ObservedVoiceChannelOptionsValidator>();

        private static IServiceCollection ConfigureOptions(this IServiceCollection serviceCollection)
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            IConfiguration configuration = serviceProvider.GetService<IConfiguration>();
            
            IConfigurationSection botConfigurationSection = configuration.GetSection("Bot");
            serviceCollection.Configure<BotOptions>(botConfigurationSection);
            
            IConfigurationSection botCommandsConfigurationSection = configuration.GetSection("BotCommands");
            serviceCollection.Configure<BotCommandOptions>(botCommandsConfigurationSection);
            
            IConfigurationSection observedVoiceChannelOptionsSection = configuration.GetSection("ObservedVoiceChannel");
            serviceCollection.Configure<ObservedVoiceChannelOptions>(observedVoiceChannelOptionsSection);
            
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
                .AddTransient<ReadyEventHandler>()
                .AddTransient<ConnectedEventHandler>()
                .AddTransient<DisconnectedEventHandler>()
                .AddTransient<LogEventHandler>()
                .AddTransient<LoggedInEventHandler>()
                .AddTransient<LoggedOutEventHandler>()
                .AddTransient<MessageReceivedEventHandler>()
                .AddTransient<ChannelDestroyedEventHandler>()
                .AddTransient<ReactionAddedEventHandler>();
        
        private static IServiceCollection ConfigureHostedServices(this IServiceCollection serviceCollection) 
            => serviceCollection
                .AddHostedService<OnReadyHostedService>()
                .AddHostedService<OnConnectedHostedService>()
                .AddHostedService<OnDisconnectedHostedService>()
                .AddHostedService<OnLogHostedService>()
                .AddHostedService<OnLoggedInHostedService>()
                .AddHostedService<OnLoggedOutHostedService>()
                .AddHostedService<OnMessageReceivedHostedService>()
                .AddHostedService<OnChannelDestroyedHostedService>()
                .AddHostedService<OnReactionAddedHostedService>();
    }
}