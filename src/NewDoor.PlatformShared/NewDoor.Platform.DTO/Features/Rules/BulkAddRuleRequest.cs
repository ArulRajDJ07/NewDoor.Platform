    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Rules.Models
    {
        public class BulkAddRuleRequest  
        {
           public ICollection<AddRuleRequest> ruleList { get; set; }
        }
    }