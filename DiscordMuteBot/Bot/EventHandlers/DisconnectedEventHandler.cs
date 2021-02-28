using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bot.EventHandlers
{
    public sealed class DisconnectedEventHandler
    {
        private readonly ILogger<DisconnectedEventHandler> _logger;
        
        public DisconnectedEventHandler(ILogger<DisconnectedEventHandler> logger)
        {
            _logger = logger;
        }
        
        public Task OnDisconnected(Exception exception)
        {
            _logger.LogError(exception.Message);
            
            return Task.CompletedTask;
        }
    }
}