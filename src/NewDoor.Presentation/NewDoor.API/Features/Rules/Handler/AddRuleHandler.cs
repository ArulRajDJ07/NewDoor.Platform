    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Rules.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Rules.Command;

    namespace NewDoor.API.Features.Rules.Handler
    {
        public class AddRuleHandler(IMapper mapper, IRuleRepository ruleRepository)
            : BaseAddHandler<AddRuleCommand, AddRuleRequest, Rule, IRuleRepository, RuleResponse>(mapper, ruleRepository);
    }