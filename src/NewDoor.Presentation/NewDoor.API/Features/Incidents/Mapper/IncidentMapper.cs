    using AutoMapper;
    using NewDoor.Platform.DTO.Features.Incidents.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.Incidents.Mapper
    {
        public class IncidentMapper : Profile
        {
            public IncidentMapper()
            {
                CreateMap<AddIncidentRequest, Incident>();
                CreateMap<Incident, IncidentResponse>();
            }
        }
    }