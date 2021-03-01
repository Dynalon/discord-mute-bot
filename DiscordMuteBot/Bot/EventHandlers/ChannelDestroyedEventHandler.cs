using System.Threading.Tasks;
using Discord.WebSocket;

namespace Bot.EventHandlers
{
    public sealed class ChannelDestroyedEventHandler
    {
        private readonly ObservedVoiceChannelsCache _observedVoiceChannelsCache;
        
        public ChannelDestroyedEventHandler(ObservedVoiceChannelsCache observedVoiceChannelsCache)
        {
            _observedVoiceChannelsCache = observedVoiceChannelsCache;
        }
        
        public Task OnChannelDestroyed(SocketChannel socketChannel)
        {
            _observedVoiceChannelsCache.Remove(socketChannel.Id);
            
            return Task.CompletedTask;
        }
    }
}