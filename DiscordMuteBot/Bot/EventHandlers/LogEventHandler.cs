using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Bot.EventHandlers
{
    public sealed class LogEventHandler
    {
        private readonly ILogger<LogEventHandler> _logger;
        
        public LogEventHandler(ILogger<LogEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task OnLog(LogMessage logMessage)
        {
            _logger.LogInformation(logMessage.Message);
            
            return Task.CompletedTask;
        }
    }
}