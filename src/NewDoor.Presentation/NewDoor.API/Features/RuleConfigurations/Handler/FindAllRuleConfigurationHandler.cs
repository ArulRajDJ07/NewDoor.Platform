    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.RuleConfigurations.Query;

    namespace NewDoor.API.Features.RuleConfigurations.Handler
    {
        public class FindAllRuleConfigurationHandler : IRequestHandler<FindAllRuleConfigurationQuery, List<RuleConfigurationResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IRuleConfigurationRepository _ruleConfigurationRepository;

            public FindAllRuleConfigurationHandler(IMapper mapper, IRuleConfigurationRepository ruleConfigurationRepository)
            {
                _mapper = mapper;
                _ruleConfigurationRepository = ruleConfigurationRepository;
            }

            public async Task<List<RuleConfigurationResponse>> Handle(FindAllRuleConfigurationQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new RuleConfigurationFilterRequest();
                var ruleConfigurations = await _ruleConfigurationRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<RuleConfigurationResponse>>(ruleConfigurations);
            }
        }
    }