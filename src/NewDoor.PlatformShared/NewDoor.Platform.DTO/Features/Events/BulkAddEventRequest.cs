    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Events.Models
    {
        public class BulkAddEventRequest  
        {
           public ICollection<AddEventRequest> eventList { get; set; }
        }
    }