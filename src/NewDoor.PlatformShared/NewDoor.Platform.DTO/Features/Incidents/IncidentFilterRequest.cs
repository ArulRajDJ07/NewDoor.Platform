using NewDoor.Platform.DTO.Common;

namespace NewDoor.Platform.DTO.Features.Incidents.Models
{
    public class IncidentFilterRequest : FilterCriteria
    {
        public int? Id { get; set; }
        public int? MinId { get; set; }
        public int? MaxId { get; set; }
        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
    }
}
