    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Alarms.Models
    {
        public class BulkAddAlarmRequest  
        {
           public ICollection<AddAlarmRequest> alarmList { get; set; }
        }
    }