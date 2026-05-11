    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.Incidents.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Incidents.Query;

    namespace NewDoor.API.Features.Incidents.Handler
    {
        public class FindAllIncidentHandler : IRequestHandler<FindAllIncidentQuery, List<IncidentResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIncidentRepository _incidentRepository;

            public FindAllIncidentHandler(IMapper mapper, IIncidentRepository incidentRepository)
            {
                _mapper = mapper;
                _incidentRepository = incidentRepository;
            }

            public async Task<List<IncidentResponse>> Handle(FindAllIncidentQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new IncidentFilterRequest();
                var incidents = await _incidentRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<IncidentResponse>>(incidents);
            }
        }
    }