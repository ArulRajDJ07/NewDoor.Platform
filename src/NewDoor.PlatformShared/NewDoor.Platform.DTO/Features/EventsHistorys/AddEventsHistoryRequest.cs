    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.EventsHistorys.Models
    {
        public class AddEventsHistoryRequest  
        {
            public int? EventId { get; set; }

            public int? DeviceId { get; set; }

            [Required]
            [MaxLength(50)]
            public string EventType { get; set; } = string.Empty;

            [MaxLength(20)]
            public string Severity { get; set; } = string.Empty;

            [MaxLength(500)]
            public string ProcessingResult { get; set; } = string.Empty;

            [MaxLength(100)]
            public string ProcessorName { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string Remarks { get; set; } = string.Empty;

            public DateTime ProcessedUtc { get; set; }
        }
    }