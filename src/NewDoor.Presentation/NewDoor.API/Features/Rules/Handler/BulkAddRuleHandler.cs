using AutoMapper;
using MediatR;
using NewDoor.API.Features.Rules.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.Rules.Handler
{
    public class BulkAddRuleHandler(IMapper mapper, IRuleRepository ruleRepository) 
        : IRequestHandler<BulkAddRuleCommand, int>
    {
        public async Task<int> Handle(BulkAddRuleCommand request, CancellationToken cancellationToken)
        {
            var rules = request.ruleRequest.ruleList.Select(r => mapper.Map<Rule>(r)).ToList();
            return await ruleRepository.AddRangeAsync(rules);
        }
    }
}
