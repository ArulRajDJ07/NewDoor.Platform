    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.RuleConfigurations.Models
    {
        public class RuleConfigurationResponse : BaseModel
        {
            public int Id { get; set; }

            public int RuleId { get; set; }

            public string ConfigKey { get; set; } = string.Empty;

            public string ConfigValue { get; set; } = string.Empty;

            public string ConfigType { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public string Unit { get; set; } = string.Empty;

            public bool IsActive { get; set; }
        }
    }