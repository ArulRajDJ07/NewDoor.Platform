    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Devices.Models
    {
        public class AddDeviceRequest  
        {
    [Required]
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceType { get; set; }

        [Required]
        public int BuildingId { get; set; }

        
        public string Floor { get; set; }

        
        public string Zone { get; set; }

        
        public string FirmwareVersion { get; set; }

        [Required]
        public string Status { get; set; }

        
        public DateTime CreatedOn { get; set; }

        
        public DateTime UpdatedOn { get; set; }
        }
    }