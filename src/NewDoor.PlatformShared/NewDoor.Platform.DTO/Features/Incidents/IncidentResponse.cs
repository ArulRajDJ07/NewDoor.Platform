    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.Incidents.Models
    {
        public class IncidentResponse : BaseModel
        {
            public int Id { get; set; }

            public string IncidentCode { get; set; } = string.Empty;

            public int BuildingId { get; set; }

            public string IncidentType { get; set; } = string.Empty;

            public string Severity { get; set; } = string.Empty;

            public string Status { get; set; } = string.Empty;

            public DateTime StartedUtc { get; set; }

            public DateTime? EndedUtc { get; set; }

            public string Summary { get; set; } = string.Empty;

            public string RootCause { get; set; } = string.Empty;

            public bool TriggeredByRule { get; set; }

            public int EventCount { get; set; }
        }
    }