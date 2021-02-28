using System.Threading.Tasks;
using Discord.WebSocket;

namespace Bot.EventHandlers
{
    public sealed class ChannelDestroyedEventHandler
    {
        public Task OnChannelDestroyed(SocketChannel socketChannel)
        {
            ObservedVoiceChannelsCache.Release(socketChannel.Id);
            
            return Task.CompletedTask;
        }
    }
}