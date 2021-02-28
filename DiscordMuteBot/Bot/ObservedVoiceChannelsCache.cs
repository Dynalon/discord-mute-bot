using System.Collections.Generic;

namespace Bot
{
    public static class ObservedVoiceChannelsCache
    {
        private static readonly Dictionary<ulong, bool> _observedVoiceChannels = new Dictionary<ulong, bool>();

        public static bool IsObserved(ulong voiceChannelId) => _observedVoiceChannels.ContainsKey(voiceChannelId);
        public static void Observe(ulong voiceChannelId) => _observedVoiceChannels.Add(voiceChannelId, false);
        public static bool Release(ulong voiceChannelId) => _observedVoiceChannels.Remove(voiceChannelId);
    }
}