    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Events.Models
    {
        public class AddEventRequest  
        {
            [Required]
            [MaxLength(100)]
            public string EventId { get; set; } = string.Empty;

            [Required]
            public int DeviceId { get; set; }

            [Required]
            public int BuildingId { get; set; }

            [Required]
            [MaxLength(50)]
            public string EventType { get; set; } = string.Empty;

            public double Temperature { get; set; }

            public double SmokeLevel { get; set; }

            public double BatteryLevel { get; set; }

            public double SignalStrength { get; set; }

            [MaxLength(200)]
            public string Payload { get; set; } = string.Empty;

            [MaxLength(20)]
            public string Severity { get; set; } = string.Empty;

            public DateTime EventUtc { get; set; }

            [MaxLength(100)]
            public string CorrelationId { get; set; } = string.Empty;
        }
    }