using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Options;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;

namespace Bot.Commands
{
    public sealed class ObserveVoiceChannelCommand : ModuleBase
    {
        private readonly IOptionsMonitor<ObservedVoiceChannelOptions> _observedVoiceChannelOptionsMonitor;
        
        public ObserveVoiceChannelCommand(IOptionsMonitor<ObservedVoiceChannelOptions> observedVoiceChannelOptionsMonitor)
        {
            _observedVoiceChannelOptionsMonitor = observedVoiceChannelOptionsMonitor;
        }
        
        [Command("observe")]
        [Alias("o")]
        [RequireContext(ContextType.Guild)]
        [Summary("Observes a voice channel")]
        public async Task ObserveVoiceChannel([Summary("The id of the voice channel to observe")] ulong voiceChannelId)
        {
            if (ObservedVoiceChannelsCache.IsObserved(voiceChannelId))
            {
                await Context.Channel.SendMessageAsync($"Voice channel with id {voiceChannelId} is already under observation.");
            }
            else
            {
                IReadOnlyCollection<IVoiceChannel> guildVoiceChannels = await Context.Guild.GetVoiceChannelsAsync();
                IVoiceChannel voiceChannel = guildVoiceChannels.FirstOrDefault(currentVoiceChannel => currentVoiceChannel.Id == voiceChannelId);

                if (voiceChannel == null)
                {
                    await Context.Channel.SendMessageAsync($"Voice channel with id {voiceChannelId} not found on this guild.");
                }
                else
                {
                    ObservedVoiceChannelsCache.Observe(voiceChannelId);
                    
                    IUserMessage message = await Context.Channel.SendMessageAsync($"Observing voice channel with id {voiceChannelId}.");
                    
                    IEmote emote = new Emoji(_observedVoiceChannelOptionsMonitor.CurrentValue.MutedEmoji);
                    await message.AddReactionAsync(emote);
                }
            }
        }
    }
}