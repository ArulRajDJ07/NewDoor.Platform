    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.Alarms.Models
    {
        public class AlarmResponse : BaseModel
        {
            public int Id { get; set; }

            public string AlarmCode { get; set; } = string.Empty;

            public int DeviceId { get; set; }

            public int BuildingId { get; set; }

            public int RuleId { get; set; }

            public int? IncidentId { get; set; }

            public string Severity { get; set; } = string.Empty;

            public string AlarmMessage { get; set; } = string.Empty;

            public string AlarmStatus { get; set; } = string.Empty;

            public DateTime TriggeredUtc { get; set; }

            public DateTime? AcknowledgedUtc { get; set; }

            public DateTime? ResolvedUtc { get; set; }

            public string TriggeredBy { get; set; } = string.Empty;

            public string ResolutionNotes { get; set; } = string.Empty;
        }
    }