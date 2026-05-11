    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Incidents.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Incidents.Query;

    namespace NewDoor.API.Features.Incidents.Handler
    {
        public class FindAllIncidentHandler(IMapper mapper, IIncidentRepository incidentRepository)
            : FindAllHandler<FindAllIncidentQuery, IncidentResponse, Incident, IIncidentRepository>(mapper, incidentRepository);
    }