    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Alarm : BaseEntity
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string AlarmCode { get; set; } = string.Empty;

            [Required]
            public int DeviceId { get; set; }

            [Required]
            public int BuildingId { get; set; }

            [Required]
            public int RuleId { get; set; }

            public int? IncidentId { get; set; }

            [MaxLength(20)]
            public string Severity { get; set; } = string.Empty;

            [MaxLength(500)]
            public string AlarmMessage { get; set; } = string.Empty;

            [MaxLength(20)]
            public string AlarmStatus { get; set; } = string.Empty;

            public DateTime TriggeredUtc { get; set; }

            public DateTime? AcknowledgedUtc { get; set; }

            public DateTime? ResolvedUtc { get; set; }

            [MaxLength(200)]
            public string TriggeredBy { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string ResolutionNotes { get; set; } = string.Empty;

            // Navigation properties
            public virtual Device Device { get; set; } = null!;
            public virtual Building Building { get; set; } = null!;
            public virtual Rule Rule { get; set; } = null!;
            public virtual Incident? Incident { get; set; }
        }
    }