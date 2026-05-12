    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Incidents.Models
    {
        public class BulkAddIncidentRequest  
        {
           public ICollection<AddIncidentRequest> incidentList { get; set; }
        }
    }