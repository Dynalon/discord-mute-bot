using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Bot.EventHandlers
{
    public sealed class UserVoiceStateUpdatedEventHandler
    {
        private readonly ObservedVoiceChannelsCache _observedVoiceChannelsCache;
        
        public UserVoiceStateUpdatedEventHandler(ObservedVoiceChannelsCache observedVoiceChannelsCache)
        {
            _observedVoiceChannelsCache = observedVoiceChannelsCache;
        }
        
        public async Task OnUserVoiceStateUpdated(
            SocketUser socketUser, 
            SocketVoiceState oldSocketVoiceState,
            SocketVoiceState newSocketVoiceState)
        {
            if (socketUser is SocketGuildUser socketGuildUser)
            {
                bool userIsMuted = socketGuildUser.VoiceState?.IsMuted == true;
                bool userIsNotOffline = socketGuildUser.Status != UserStatus.Offline;
                
                // user left observed muted voice channel
                if (oldSocketVoiceState.VoiceChannel != null && 
                    newSocketVoiceState.VoiceChannel == null &&
                    _observedVoiceChannelsCache.TryGetValue(oldSocketVoiceState.VoiceChannel.Id, out ObservedVoiceChannel observedLeftVoiceChannel) &&
                    observedLeftVoiceChannel.IsMuted &&
                    userIsMuted &&
                    userIsNotOffline
                    )
                {
                    await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = false);
                }
                // user joined observed muted voice channel
                else if (oldSocketVoiceState.VoiceChannel == null && 
                         newSocketVoiceState.VoiceChannel != null &&
                         _observedVoiceChannelsCache.TryGetValue(newSocketVoiceState.VoiceChannel.Id, out ObservedVoiceChannel observedJoinedVoiceChannel) &&
                         observedJoinedVoiceChannel.IsMuted &&
                         !userIsMuted &&
                         userIsNotOffline)
                {
                    await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = true);
                }
                // user changed voice channels
                else if (oldSocketVoiceState.VoiceChannel != null && 
                         newSocketVoiceState.VoiceChannel != null &&
                         userIsNotOffline)
                {
                    bool oldVoiceChannelObserved = _observedVoiceChannelsCache.TryGetValue(
                        oldSocketVoiceState.VoiceChannel.Id, out ObservedVoiceChannel oldObservedVoiceChannel);
                    
                    bool newVoiceChannelObserved = _observedVoiceChannelsCache.TryGetValue(
                        newSocketVoiceState.VoiceChannel.Id, out ObservedVoiceChannel newObservedVoiceChannel);

                    // user moved from observed muted voice channel to unobserved voice channel
                    if (oldVoiceChannelObserved && 
                        !newVoiceChannelObserved &&
                        oldObservedVoiceChannel.IsMuted &&
                        userIsMuted)
                    {
                        await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = false);
                    }
                    // user moved from unobserved voice channel to observed muted voice channel
                    else if (!oldVoiceChannelObserved && 
                             newVoiceChannelObserved &&
                             newObservedVoiceChannel.IsMuted &&
                             !userIsMuted)
                    {
                        await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = true);
                    }
                    // both voice channels are observed
                    else if (oldVoiceChannelObserved && newVoiceChannelObserved)
                    {
                        // user moved from muted to unmuted voice channel
                        if (oldObservedVoiceChannel.IsMuted && 
                            !newObservedVoiceChannel.IsMuted &&
                            userIsMuted)
                        {
                            await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = false);
                        }
                        // user moved from unmuted to muted voice channel
                        else if (!oldObservedVoiceChannel.IsMuted && 
                                 newObservedVoiceChannel.IsMuted && 
                                 !userIsMuted)
                        {
                            await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = true);
                        }
                        // user moved from muted to muted voice channel
                        else if (oldObservedVoiceChannel.IsMuted && 
                                 newObservedVoiceChannel.IsMuted && 
                                 !userIsMuted)
                        {
                            await socketGuildUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = true);
                        }
                    }
                }
            }
        }
    }
}