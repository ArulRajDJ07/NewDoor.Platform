using NewDoor.Platform.DTO.Common;

namespace NewDoor.Platform.DTO.Features.Events.Models
{
    public class EventFilterRequest : FilterCriteria
    {
        public int? Id { get; set; }
        public int? MinId { get; set; }
        public int? MaxId { get; set; }
        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
    }
}
