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
        private readonly ObservedVoiceChannelsCache _observedVoiceChannelsCache;
        
        public ObserveVoiceChannelCommand(
            IOptionsMonitor<ObservedVoiceChannelOptions> observedVoiceChannelOptionsMonitor,
            ObservedVoiceChannelsCache observedVoiceChannelsCache)
        {
            _observedVoiceChannelOptionsMonitor = observedVoiceChannelOptionsMonitor;
            _observedVoiceChannelsCache = observedVoiceChannelsCache;
        }
        
        [Command("observe")]
        [Alias("o")]
        [RequireContext(ContextType.Guild)]
        [Summary("Observes a voice channel")]
        public async Task ObserveVoiceChannel([Summary("The id of the voice channel to observe")] ulong voiceChannelId)
        {
            IReadOnlyCollection<IVoiceChannel> guildVoiceChannels = await Context.Guild.GetVoiceChannelsAsync();
            IVoiceChannel voiceChannel = guildVoiceChannels.FirstOrDefault(currentVoiceChannel => currentVoiceChannel.Id == voiceChannelId);

            if (voiceChannel == null)
            {
                await Context.Channel.SendMessageAsync($"Voice channel with id {voiceChannelId} not found on this guild.");
            }
            else if (_observedVoiceChannelsCache.ContainsKey(voiceChannelId))
            {
                await Context.Channel.SendMessageAsync($"Voice channel {voiceChannel.Name} is already under observation.");
            }
            else
            {
                IUserMessage message = await Context.Channel.SendMessageAsync($"Observing voice channel {voiceChannel.Name}.");
                    
                IEmote emote = new Emoji(_observedVoiceChannelOptionsMonitor.CurrentValue.MutedEmoji);
                await message.AddReactionAsync(emote);
                
                _observedVoiceChannelsCache.Add(voiceChannelId, new ObservedVoiceChannel()
                {
                    VoiceChannelId = voiceChannelId,
                    IsMuted = false,
                    MessageId = message.Id
                });
            }
        }
    }
}