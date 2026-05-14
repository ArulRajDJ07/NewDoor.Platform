    using DoWhatta.Platform.DTO.Model;
    using NewDoor.Platform.DTO.Features.RuleConfigurations.Models;

    namespace NewDoor.Platform.DTO.Features.Rules.Models
    {
        public class RuleResponse : BaseModel
        {
            public int Id { get; set; }

            public string RuleCode { get; set; } = string.Empty;

            public string RuleName { get; set; } = string.Empty;

            public string RuleType { get; set; } = string.Empty;

            public string DeviceType { get; set; } = string.Empty;

            public double ThresholdValue { get; set; }

            public int WindowSeconds { get; set; }

            public string Severity { get; set; } = string.Empty;

            public bool IsActive { get; set; }

            public string Description { get; set; } = string.Empty;

            public List<RuleConfigurationResponse> Configurations { get; set; } = new List<RuleConfigurationResponse>();
        }
    }