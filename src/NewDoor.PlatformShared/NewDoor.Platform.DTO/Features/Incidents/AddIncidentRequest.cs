    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Incidents.Models
    {
        public class AddIncidentRequest  
        {
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

            [MaxLength(1000)]
            public string Summary { get; set; } = string.Empty;

            [MaxLength(500)]
            public string RootCause { get; set; } = string.Empty;

            public bool TriggeredByRule { get; set; }

            public int EventCount { get; set; }
        }
    }