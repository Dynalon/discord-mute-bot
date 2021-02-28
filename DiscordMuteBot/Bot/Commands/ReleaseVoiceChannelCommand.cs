using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Commands
{
    public sealed class ReleaseVoiceChannelCommand : ModuleBase
    {
        [Command("release")]
        [Alias("r")]
        [RequireContext(ContextType.Guild)]
        [Summary("Releases a voice channel from observation")]
        public async Task ReleaseVoiceChannel([Summary("The id of the observed voice channel")] ulong voiceChannelId)
        {
            IReadOnlyCollection<IVoiceChannel> guildVoiceChannels = await Context.Guild.GetVoiceChannelsAsync();
            IVoiceChannel voiceChannel = guildVoiceChannels.FirstOrDefault(currentVoiceChannel => currentVoiceChannel.Id == voiceChannelId);
            
            if (voiceChannel == null)
            {
                await Context.Channel.SendMessageAsync($"Voice channel with id {voiceChannelId} not found on this guild.");
            }
            else if (!ObservedVoiceChannelsCache.TryGetVoiceChannelMuteState(voiceChannelId, out bool voiceChannelIsMuted))
            {
                await Context.Channel.SendMessageAsync($"Voice channel {voiceChannel.Name} hasn't been observed.");
            }
            else
            {
                if (voiceChannelIsMuted)
                {
                    IEnumerable<IGuildUser> voiceChannelUsers = await voiceChannel
                        .GetUsersAsync()
                        .FlattenAsync();
                    
                    IEnumerable<Task> unmuteTasks = voiceChannelUsers.Select(voiceChannelUser =>
                        voiceChannelUser.ModifyAsync(guildUserProperties => guildUserProperties.Mute = false));
                    
                    await Task.WhenAll(unmuteTasks);
                }

                ObservedVoiceChannelsCache.Release(voiceChannelId);
                
                await Context.Channel.SendMessageAsync($"Voice channel {voiceChannel.Name} has been released from observation.");
            }
        }
    }
}