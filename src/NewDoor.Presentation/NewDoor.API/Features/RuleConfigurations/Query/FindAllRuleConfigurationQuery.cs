    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;

    namespace NewDoor.API.Features.RuleConfigurations.Query
    {
        public record FindAllRuleConfigurationQuery(RuleConfigurationFilterRequest? Filter = null) : BaseFindAllQuery<RuleConfigurationResponse>;
    }