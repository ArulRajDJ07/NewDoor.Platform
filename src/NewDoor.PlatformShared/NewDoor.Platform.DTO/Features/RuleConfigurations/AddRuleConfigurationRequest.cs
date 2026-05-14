    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.RuleConfigurations.Models
    {
        public class AddRuleConfigurationRequest  
        {
            [Required]
            public int RuleId { get; set; }

            [Required]
            [MaxLength(50)]
            public string ConfigKey { get; set; } = string.Empty;

            [Required]
            [MaxLength(200)]
            public string ConfigValue { get; set; } = string.Empty;

            [MaxLength(50)]
            public string Unit { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }