    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using NewDoor.Platform.DTO.Features.Rules.Models;
    using NewDoor.API.Features.Rules.Command;
    using NewDoor.API.Features.Rules.Query;

    [Route("api/[controller]")]
    [ApiController]
    public class RuleController(IMediator mediator) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<List<RuleResponse>> GetAll() =>
            await mediator.Send(new FindAllRuleQuery());

        [HttpPost]
        public async Task<RuleResponse> Create([FromBody] AddRuleRequest request) =>
            await mediator.Send(new AddRuleCommand(request));

        [HttpDelete("{id}")]
        public async Task<long> Delete(long id) =>
            await mediator.Send(new DeleteRuleCommand(id));

        [HttpPost("Rule/bulk")]
        public async Task<ActionResult<BulkAddRuleRequest >> CreateRules([FromBody] BulkAddRuleRequest Rulerequests)
        {
            var result = await mediator.Send(new BulkAddRuleCommand(Rulerequests));
            return Ok(result);
        }
    }