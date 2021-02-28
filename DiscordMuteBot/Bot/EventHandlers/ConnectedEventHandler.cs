using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bot.EventHandlers
{
    public sealed class ConnectedEventHandler
    {
        private readonly ILogger<ConnectedEventHandler> _logger;
        
        public ConnectedEventHandler(ILogger<ConnectedEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task OnConnected()
        {
            _logger.LogInformation("Connected");
            
            return Task.CompletedTask;
        }
    }
}