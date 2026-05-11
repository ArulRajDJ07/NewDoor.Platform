    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.Rules.Models;

    namespace NewDoor.API.Features.Rules.Query
    {
        public record FindAllRuleQuery : BaseFindAllQuery<RuleResponse>;
    }