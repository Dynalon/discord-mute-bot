using Discord;
using Discord.Commands;

namespace Bot.Options
{
    public sealed class BotCommandOptions
    {
        public RunMode DefaultRunMode { get; set; }
        public bool CaseSensitiveCommands { get; set; }
        public LogSeverity LogSeverity { get; set; }
        public char Prefix { get; set; }
    }
}