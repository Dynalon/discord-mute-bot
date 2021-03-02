using System.Threading.Tasks;
using Discord.WebSocket;

namespace Bot.EventHandlers
{
    public sealed class UserVoiceStateUpdatedEventHandler
    {
        public async Task OnUserVoiceStateUpdated(
            SocketUser socketUser, 
            SocketVoiceState oldSocketVoiceState,
            SocketVoiceState newSocketVoiceState)
        {
            if (socketUser is SocketGuildUser socketGuildUser)
            {
                /*
                    TODO
                    
                    If observed voice channel is muted
                    
                        mute if a new user joins
                        unmute if a user leaves
                */
            }
        }
    }
}