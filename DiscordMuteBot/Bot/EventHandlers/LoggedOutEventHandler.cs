using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bot.EventHandlers
{
    public sealed class LoggedOutEventHandler
    {
        private readonly ILogger<LoggedOutEventHandler> _logger;
        
        public LoggedOutEventHandler(ILogger<LoggedOutEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task OnLoggedOut()
        {
            _logger.LogInformation("Logged out");
            
            return Task.CompletedTask;
        }
    }
}