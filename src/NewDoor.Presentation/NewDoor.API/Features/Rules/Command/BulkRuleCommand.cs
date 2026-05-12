    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Rules.Models;

    namespace NewDoor.API.Features.Rules.Command
    {
        public record BulkAddRuleCommand(BulkAddRuleRequest ruleRequest)
            : BaseAddCommand<BulkAddRuleRequest, int>(ruleRequest);
    }