    using DoWhatta.Platform.Data.Mediator.Queries;
    using NewDoor.Platform.DTO.Features.Incidents.Models;

    namespace NewDoor.API.Features.Incidents.Query
    {
        public record FindAllIncidentQuery : BaseFindAllQuery<IncidentResponse>;
    }