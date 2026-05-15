    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.Events.Models
    {
        public class EventResponse : BaseModel
        {
            public int Id { get; set; }

            public string EventId { get; set; } = string.Empty;

            public string DeviceId { get; set; } = string.Empty;

            public int BuildingId { get; set; }

            public string EventType { get; set; } = string.Empty;

            public double Temperature { get; set; }

            public double SmokeLevel { get; set; }

            public double BatteryLevel { get; set; }

            public double SignalStrength { get; set; }

            public string Payload { get; set; } = string.Empty;

            public string Severity { get; set; } = string.Empty;

            public DateTime EventUtc { get; set; }

            public DateTime ProcessedUtc { get; set; }

            public string Status { get; set; } = string.Empty;

            public string CorrelationId { get; set; } = string.Empty;
        }
    }