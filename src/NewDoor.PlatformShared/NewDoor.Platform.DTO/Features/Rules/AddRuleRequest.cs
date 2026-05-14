    using System.ComponentModel.DataAnnotations;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;

    namespace NewDoor.Platform.DTO.Features.Rules.Models
    {
        public class AddRuleRequest  
        {
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

            [Required]
            public double ThresholdValue { get; set; }

            [Required]
            public int WindowSeconds { get; set; }

            [MaxLength(20)]
            public string Severity { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;

            [MaxLength(500)]
            public string Description { get; set; } = string.Empty;

            // List of configurations for this rule
            public List<AddRuleConfigurationRequest> Configurations { get; set; } = new List<AddRuleConfigurationRequest>();
        }
    }