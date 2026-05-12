    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
    using NewDoor.API.Features.RuleConfigurations.Command;
    using NewDoor.API.Features.RuleConfigurations.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class RuleConfigurationController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<RuleConfigurationResponse>> GetAll([FromQuery] RuleConfigurationFilterRequest? filter) =>
            await mediator.Send(new FindAllRuleConfigurationQuery(filter));

        [HttpPost]
        public async Task<RuleConfigurationResponse> Create([FromBody] AddRuleConfigurationRequest request) =>
            await mediator.Send(new AddRuleConfigurationCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteRuleConfigurationCommand(id));

        [HttpPost("RuleConfiguration/bulk")]
        public async Task<ActionResult<BulkAddRuleConfigurationRequest >> CreateRuleConfigurations([FromBody] BulkAddRuleConfigurationRequest RuleConfigurationrequests)
        {
            var result = await mediator.Send(new BulkAddRuleConfigurationCommand(RuleConfigurationrequests));
            return Ok(result);
        }
    }