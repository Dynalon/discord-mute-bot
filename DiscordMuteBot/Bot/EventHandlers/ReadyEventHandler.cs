using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bot.EventHandlers
{
    public sealed class ReadyEventHandler
    {
        private readonly ILogger<ReadyEventHandler> _logger;
        
        public ReadyEventHandler(ILogger<ReadyEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task OnReady()
        {
            _logger.LogInformation("Ready");
            
            return Task.CompletedTask;
        }
    }
}