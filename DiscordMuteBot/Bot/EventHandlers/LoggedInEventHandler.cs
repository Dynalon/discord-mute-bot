using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bot.EventHandlers
{
    public sealed class LoggedInEventHandler
    {
        private readonly ILogger<LoggedInEventHandler> _logger;
        
        public LoggedInEventHandler(ILogger<LoggedInEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task OnLoggedIn()
        {
            _logger.LogInformation("Logged in");
            
            return Task.CompletedTask;
        }
    }
}