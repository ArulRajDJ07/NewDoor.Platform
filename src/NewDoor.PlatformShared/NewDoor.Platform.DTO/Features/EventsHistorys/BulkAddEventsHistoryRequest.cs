    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.EventsHistorys.Models
    {
        public class BulkAddEventsHistoryRequest  
        {
           public ICollection<AddEventsHistoryRequest> eventsHistoryList { get; set; }
        }
    }