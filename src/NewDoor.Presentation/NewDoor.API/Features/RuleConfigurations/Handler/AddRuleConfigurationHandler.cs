    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.RuleConfigurations.Command;

    namespace NewDoor.API.Features.RuleConfigurations.Handler
    {
        public class AddRuleConfigurationHandler(IMapper mapper, IRuleConfigurationRepository ruleConfigurationRepository)
            : BaseAddHandler<AddRuleConfigurationCommand, AddRuleConfigurationRequest, RuleConfiguration, IRuleConfigurationRepository, RuleConfigurationResponse>(mapper, ruleConfigurationRepository);
    }