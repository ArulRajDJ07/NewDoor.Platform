    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Incidents.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Incidents.Command;

    namespace NewDoor.API.Features.Incidents.Handler
    {
        public class AddIncidentHandler(IMapper mapper, IIncidentRepository incidentRepository)
            : BaseAddHandler<AddIncidentCommand, AddIncidentRequest, Incident, IIncidentRepository, IncidentResponse>(mapper, incidentRepository);
    }