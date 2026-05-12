    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.Devices.Models
    {
        public class BulkAddDeviceRequest  
        {
           public ICollection<AddDeviceRequest> deviceList { get; set; }
        }
    }