    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Alarms.Models
    {
        public class AddAlarmRequest  
        {
            [Required]
            [MaxLength(100)]
            public string AlarmCode { get; set; } = string.Empty;

            [MaxLength(100)]
            public string DeviceId { get; set; } = string.Empty;

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

            [MaxLength(200)]
            public string TriggeredBy { get; set; } = string.Empty;
        }
    }