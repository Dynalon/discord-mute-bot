using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Options;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Bot.EventHandlers
{
    public sealed class ReactionAddedEventHandler
    { 
        private readonly ObservedVoiceChannelsCache _observedVoiceChannelsCache;
        private readonly IOptionsMonitor<ObservedVoiceChannelOptions> _observedVoiceChannelOptionsMonitor;
        
        public ReactionAddedEventHandler(ObservedVoiceChannelsCache observedVoiceChannelsCache, IOptionsMonitor<ObservedVoiceChannelOptions> observedVoiceChannelOptionsMonitor)
        {
            _observedVoiceChannelsCache = observedVoiceChannelsCache;
            _observedVoiceChannelOptionsMonitor = observedVoiceChannelOptionsMonitor;
        }
        
        public async Task OnReactionAdded(
            Cacheable<IUserMessage, ulong> cacheable, 
            ISocketMessageChannel socketMessageChannel, 
            SocketReaction socketReaction)
        {
            ObservedVoiceChannelOptions observedVoiceChannelOptions = _observedVoiceChannelOptionsMonitor.CurrentValue;
            
            string emoteName = socketReaction.Emote.Name;
            bool emoteIsMutedEmoji = emoteName == observedVoiceChannelOptions.MutedEmoji;
            bool emoteIsUnMutedEmoji = emoteName == observedVoiceChannelOptions.UnMutedEmoji;
            
            if (emoteIsMutedEmoji || emoteIsUnMutedEmoji)
            {
                ObservedVoiceChannel observedVoiceChannel =
                    _observedVoiceChannelsCache.Values.FirstOrDefault(currentObservedVoiceChannel => 
                        currentObservedVoiceChannel.MessageId == socketReaction.MessageId);
                
                if (observedVoiceChannel != null)
                {
                    IMessage message = await socketMessageChannel.GetMessageAsync(cacheable.Id);

                    if (message.Reactions[socketReaction.Emote].ReactionCount > 1)
                    { 
                        SocketGuildChannel socketGuildChannel = socketMessageChannel as SocketGuildChannel;
                        SocketVoiceChannel socketVoiceChannel = socketGuildChannel.Guild.GetVoiceChannel(observedVoiceChannel.VoiceChannelId);
                        IVoiceChannel voiceChannel = socketVoiceChannel as IVoiceChannel;

                        IEnumerable<IGuildUser> voiceChannelUsers = await voiceChannel
                            .GetUsersAsync()
                            .FlattenAsync();
                    
                        Emoji mutedEmoji = new Emoji(observedVoiceChannelOptions.MutedEmoji);
                        Emoji unMutedEmoji = new Emoji(observedVoiceChannelOptions.UnMutedEmoji);

                        if (observedVoiceChannel.IsMuted && emoteIsUnMutedEmoji)
                        {
                            List<Task> tasks = voiceChannelUsers
                                .Where(voiceChannelUser =>
                                {
                                    SocketGuildUser socketGuildUser = voiceChannelUser as SocketGuildUser;
                                    bool userIsMuted = socketGuildUser.VoiceState?.IsMuted == true;
                                    bool userNotOffline = socketGuildUser.Status != UserStatus.Offline;
                            
                                    return userIsMuted && userNotOffline;
                                })
                                .Select(voiceChannelUser => voiceChannelUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = false))
                                .ToList();
                            
                            tasks.Add(message.RemoveAllReactionsForEmoteAsync(unMutedEmoji));
                            tasks.Add(message.AddReactionAsync(mutedEmoji));
                            
                            await Task.WhenAll(tasks);

                            observedVoiceChannel.IsMuted = false;
                        }
                        else if (!observedVoiceChannel.IsMuted && emoteIsMutedEmoji)
                        {
                            List<Task> tasks = voiceChannelUsers
                                .Where(voiceChannelUser =>
                                {
                                    SocketGuildUser socketGuildUser = voiceChannelUser as SocketGuildUser;
                                    bool userNotMuted = socketGuildUser.VoiceState?.IsMuted == false;
                                    bool userNotOffline = socketGuildUser.Status != UserStatus.Offline;
                            
                                    return userNotMuted && userNotOffline;
                                })
                                .Select(voiceChannelUser => voiceChannelUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = true))
                                .ToList();
                            
                            tasks.Add(message.RemoveAllReactionsForEmoteAsync(mutedEmoji));
                            tasks.Add(message.AddReactionAsync(unMutedEmoji));
                            
                            await Task.WhenAll(tasks);

                            observedVoiceChannel.IsMuted = true;
                        }
                    }
                }
            }
        }
    }
}