    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class EventsHistory : BaseEntity
        {
            [Key]
            public int Id { get; set; }

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

            // Navigation properties
            public virtual Event Event { get; set; } = null!;
            public virtual Device? Device { get; set; }
        }
    }