    using DoWhatta.Platform.Data.Mediator.BaseCommands;
    using NewDoor.Platform.DTO.Features.Incidents.Models;

    namespace NewDoor.API.Features.Incidents.Command
    {
        public record BulkAddIncidentCommand(BulkAddIncidentRequest incidentRequest)
            : BaseAddCommand<BulkAddIncidentRequest, int>(incidentRequest);
    }