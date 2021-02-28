using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.OptionsValidation
{
    public sealed class ObservedVoiceChannelOptionsValidator : IValidateOptions<ObservedVoiceChannelOptions>
    {
        private readonly ILogger<ObservedVoiceChannelOptionsValidator> _logger;
        
        public ObservedVoiceChannelOptionsValidator(ILogger<ObservedVoiceChannelOptionsValidator> logger)
        {
            _logger = logger;
        }
        
        public ValidateOptionsResult Validate(string name, ObservedVoiceChannelOptions options)
        {
            IList<string> validationFailures = new List<string>();

            if (string.IsNullOrEmpty(options.MutedEmoji))
            {
                validationFailures.Add($"{nameof(options.MutedEmoji)} is required.");
            }
            
            if (string.IsNullOrEmpty(options.UnMutedEmoji))
            {
                validationFailures.Add($"{nameof(options.UnMutedEmoji)} is required.");
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