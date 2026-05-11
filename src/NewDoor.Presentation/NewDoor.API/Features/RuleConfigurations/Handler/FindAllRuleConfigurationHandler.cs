    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.RuleConfigurations.Query;

    namespace NewDoor.API.Features.RuleConfigurations.Handler
    {
        public class FindAllRuleConfigurationHandler(IMapper mapper, IRuleConfigurationRepository ruleConfigurationRepository)
            : FindAllHandler<FindAllRuleConfigurationQuery, RuleConfigurationResponse, RuleConfiguration, IRuleConfigurationRepository>(mapper, ruleConfigurationRepository);
    }