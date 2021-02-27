using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.OptionsValidation
{
    public sealed class BotOptionsValidator : IValidateOptions<BotOptions>
    {
        private readonly ILogger<BotOptionsValidator> _logger;
        
        public BotOptionsValidator(ILogger<BotOptionsValidator> logger)
        {
            _logger = logger;
        }
        
        public ValidateOptionsResult Validate(string name, BotOptions options)
        {
            IList<string> validationFailures = new List<string>();

            if (string.IsNullOrEmpty(options.Token))
            {
                validationFailures.Add($"{nameof(options.Token)} is required.");
            }

            if (options.MessageCacheSize == 0)
            {
                validationFailures.Add($"{nameof(options.MessageCacheSize)} must be greater than 0.");
            }

            if (validationFailures.Any())
            {
                string validationFailureErrorMessage = string.Join(Environment.NewLine, validationFailures);
                
                _logger.LogError(validationFailureErrorMessage);
                
                return ValidateOptionsResult.Fail(validationFailures);
            }
            
            return ValidateOptionsResult.Success;
        }
    }
}