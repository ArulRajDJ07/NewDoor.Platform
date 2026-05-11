    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Rules.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Rules.Query;

    namespace NewDoor.API.Features.Rules.Handler
    {
        public class FindAllRuleHandler(IMapper mapper, IRuleRepository ruleRepository)
            : FindAllHandler<FindAllRuleQuery, RuleResponse, Rule, IRuleRepository>(mapper, ruleRepository);
    }