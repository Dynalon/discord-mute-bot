using Discord;

namespace Bot.Options
{
    public sealed class BotOptions
    {
        public string Token { get; set; }
        public uint MessageCacheSize { get; set; }
        public LogSeverity LogSeverity { get; set; }
    }
}