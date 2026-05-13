    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.EventsHistorys.Models
    {
        public class EventsHistoryResponse : BaseModel
        {
            public int Id { get; set; }

            public int EventId { get; set; }

            public int DeviceId { get; set; }

            public string EventType { get; set; } = string.Empty;

            public string Severity { get; set; } = string.Empty;

            public string ProcessingResult { get; set; } = string.Empty;

            public string ProcessorName { get; set; } = string.Empty;

            public string Remarks { get; set; } = string.Empty;

            public DateTime ProcessedUtc { get; set; }
        }
    }