    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Buildings.Models
    {
        public class BulkAddBuildingRequest  
        {
           public ICollection<AddBuildingRequest> buildingList { get; set; }
        }
    }