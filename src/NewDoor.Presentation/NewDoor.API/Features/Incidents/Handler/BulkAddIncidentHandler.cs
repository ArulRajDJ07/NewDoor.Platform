using AutoMapper;
using MediatR;
using NewDoor.API.Features.Incidents.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.Incidents.Handler
{
    public class BulkAddIncidentHandler(IMapper mapper, IIncidentRepository incidentRepository) 
        : IRequestHandler<BulkAddIncidentCommand, int>
    {
        public async Task<int> Handle(BulkAddIncidentCommand request, CancellationToken cancellationToken)
        {
            var incidents = request.incidentRequest.incidentList.Select(i => mapper.Map<Incident>(i)).ToList();
            return await incidentRepository.AddRangeAsync(incidents);
        }
    }
}
