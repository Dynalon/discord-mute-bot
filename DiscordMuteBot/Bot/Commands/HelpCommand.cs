using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Bot.Commands
{
    public sealed class HelpCommand : ModuleBase
    {
        private readonly CommandService _commandService;
        
        public HelpCommand(CommandService commandService)
        {
            _commandService = commandService;
        }
        
        [Command("help")]
        [Alias("h")]
        [Summary("Get information about commands")]
        public async Task Help()
        {
            IEnumerable<string> commandInformation = _commandService
                .Commands
                .Select(command =>
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    
                    stringBuilder.AppendLine($"**Command**: {command.Name}");
                    
                    if (command.Aliases.Count > 1)
                    {
                        string aliases = string.Join(", ", command.Aliases);
                        stringBuilder.AppendLine($"- Aliases: {aliases}");          
                    }
                    
                    stringBuilder.AppendLine($"- Summary: {command.Summary}");
                    
                    if (command.Parameters.Count > 0)
                    {
                        stringBuilder.AppendLine("- Parameters:");

                        IEnumerable<string> commandParameterInformation = command
                            .Parameters
                            .Select(parameterInfo =>
                            {
                                string parameterText = $"\t* Name: {parameterInfo.Name} | Datatype: {parameterInfo.Type} | Optional: {parameterInfo.IsOptional}";
                                
                                if (parameterInfo.IsOptional)
                                {
                                    parameterText += $" | Default: {parameterInfo.DefaultValue}"; 
                                }

                                if (!string.IsNullOrEmpty(parameterInfo.Summary))
                                {
                                    parameterText += $" | Summary: {parameterInfo.Summary}";    
                                }
                                
                                return parameterText;
                            });
                        
                        string commandParameters = string.Join(Environment.NewLine, commandParameterInformation);
                        
                        stringBuilder.AppendLine(commandParameters);
                    }
                    
                    if (command.Preconditions.Count > 0)
                    {
                        stringBuilder.AppendLine("- Preconditions:");

                        IEnumerable<string> preconditionInformation = command
                            .Preconditions
                            .Select(precondition =>
                            {
                                string preconditionText = $"\t* ";
                                
                                if (precondition is RequireContextAttribute requireContextAttribute)
                                {
                                    preconditionText += $"Required context: {requireContextAttribute.Contexts}";
                                }
                                
                                return preconditionText;
                            });
                        
                        string commandPreconditions = string.Join(Environment.NewLine, preconditionInformation);
                        
                        stringBuilder.AppendLine(commandPreconditions);
                    }
                    
                    return stringBuilder.ToString();
                });
            
            string responseMessage = string.Join(Environment.NewLine, commandInformation);
            
            await Context.Channel.SendMessageAsync(responseMessage);
        }
    }
}