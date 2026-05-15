    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Event : BaseEntity
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string EventId { get; set; } = string.Empty;

            [MaxLength(100)]
            public string DeviceId { get; set; } = string.Empty;

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

            public DateTime ProcessedUtc { get; set; }

            [MaxLength(20)]
            public string Status { get; set; } = string.Empty;

            [MaxLength(100)]
            public string CorrelationId { get; set; } = string.Empty;

            // Navigation properties
            public virtual Building Building { get; set; } = null!;
            public virtual ICollection<EventsHistory> EventsHistories { get; set; } = new List<EventsHistory>();
        }
    }