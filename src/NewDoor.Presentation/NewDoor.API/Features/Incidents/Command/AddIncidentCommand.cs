    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Incidents.Models;

    namespace NewDoor.API.Features.Incidents.Command
    {
        public record AddIncidentCommand(AddIncidentRequest incidentRequest)
            : BaseAddCommand<AddIncidentRequest, IncidentResponse>(incidentRequest);
    }