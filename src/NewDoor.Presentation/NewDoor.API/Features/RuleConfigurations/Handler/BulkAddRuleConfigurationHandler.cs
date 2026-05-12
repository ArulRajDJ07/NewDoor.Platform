using AutoMapper;
using MediatR;
using NewDoor.API.Features.RuleConfigurations.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.RuleConfigurations.Handler
{
    public class BulkAddRuleConfigurationHandler(IMapper mapper, IRuleConfigurationRepository ruleConfigurationRepository) 
        : IRequestHandler<BulkAddRuleConfigurationCommand, int>
    {
        public async Task<int> Handle(BulkAddRuleConfigurationCommand request, CancellationToken cancellationToken)
        {
            var ruleConfigurations = request.ruleConfigurationRequest.ruleConfigurationList.Select(r => mapper.Map<RuleConfiguration>(r)).ToList();
            return await ruleConfigurationRepository.AddRangeAsync(ruleConfigurations);
        }
    }
}
