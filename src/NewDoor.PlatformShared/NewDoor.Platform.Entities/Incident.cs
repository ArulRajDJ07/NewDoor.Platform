    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Incident : BaseEntity
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string IncidentCode { get; set; } = string.Empty;

            [Required]
            public int BuildingId { get; set; }

            [Required]
            [MaxLength(50)]
            public string IncidentType { get; set; } = string.Empty;

            [MaxLength(20)]
            public string Severity { get; set; } = string.Empty;

            [MaxLength(20)]
            public string Status { get; set; } = string.Empty;

            public DateTime StartedUtc { get; set; }

            public DateTime? EndedUtc { get; set; }

            [MaxLength(1000)]
            public string Summary { get; set; } = string.Empty;

            [MaxLength(500)]
            public string RootCause { get; set; } = string.Empty;

            public bool TriggeredByRule { get; set; }

            public int EventCount { get; set; }

            // Navigation properties
            public virtual Building Building { get; set; } = null!;
            public virtual ICollection<Alarm> Alarms { get; set; } = new List<Alarm>();
        }
    }