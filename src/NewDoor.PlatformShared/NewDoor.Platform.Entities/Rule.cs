    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Rule : BaseEntity
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [MaxLength(50)]
            public string RuleCode { get; set; } = string.Empty;

            [Required]
            [MaxLength(100)]
            public string RuleName { get; set; } = string.Empty;

            [Required]
            [MaxLength(50)]
            public string RuleType { get; set; } = string.Empty;

            [Required]
            [MaxLength(50)]
            public string DeviceType { get; set; } = string.Empty;

            public double ThresholdValue { get; set; }

            public int WindowSeconds { get; set; }

            [MaxLength(20)]
            public string Severity { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;

            [MaxLength(500)]
            public string Description { get; set; } = string.Empty;

            // Navigation property
            public virtual ICollection<RuleConfiguration> RuleConfigurations { get; set; } = new List<RuleConfiguration>();
        }
    }