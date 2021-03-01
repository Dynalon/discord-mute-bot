namespace Bot
{
    public sealed class ObservedVoiceChannel
    {
        public ulong VoiceChannelId { get; set; }
        public bool IsMuted { get; set; }
        public ulong MessageId { get; set; }
    }
}