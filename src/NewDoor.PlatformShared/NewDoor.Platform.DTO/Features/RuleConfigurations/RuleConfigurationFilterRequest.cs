using NewDoor.Platform.DTO.Common;

namespace NewDoor.Platform.DTO.Features.RuleConfigurations.Models
{
    public class RuleConfigurationFilterRequest : FilterCriteria
    {
        public int? Id { get; set; }
        public int? MinId { get; set; }
        public int? MaxId { get; set; }
        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
        public string? EventType { get; set; }
        public bool? IsActive { get; set; }
        public string? IncidentType { get; set; }
    }
}
