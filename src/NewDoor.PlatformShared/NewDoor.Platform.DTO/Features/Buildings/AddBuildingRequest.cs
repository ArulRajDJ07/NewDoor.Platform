    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Buildings.Models
    {
        public class AddBuildingRequest  
        {
    [Required]
        public string BuildingCode { get; set; }

        [Required]
        public string Name { get; set; }

        
        public string Address { get; set; }

        [Required]
        public string Status { get; set; }

        
        public int TotalDevices { get; set; }

        
        public int OnlineDevices { get; set; }

        
        public int OfflineDevices { get; set; }

        
        public int ActiveAlarms { get; set; }

        
        public DateTime CreatedOn { get; set; }

        
        public DateTime UpdatedOn { get; set; }
        }
    }