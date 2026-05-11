    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;

    namespace NewDoor.API.Features.RuleConfigurations.Command
    {
        public record AddRuleConfigurationCommand(AddRuleConfigurationRequest ruleConfigurationRequest)
            : BaseAddCommand<AddRuleConfigurationRequest, RuleConfigurationResponse>(ruleConfigurationRequest);
    }