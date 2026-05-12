    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.RuleConfigurations.Models
    {
        public class BulkAddRuleConfigurationRequest  
        {
           public ICollection<AddRuleConfigurationRequest> ruleConfigurationList { get; set; }
        }
    }