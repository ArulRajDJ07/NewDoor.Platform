using NewDoor.Platform.DTO.Common;

namespace NewDoor.Platform.DTO.Features.Devices.Models
{
    public class DeviceFilterRequest : FilterCriteria
    {
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
        public int? BuildingId { get; set; }
        public string? Floor { get; set; }
        public string? Zone { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
        public DateTime? UpdatedOnFrom { get; set; }
        public DateTime? UpdatedOnTo { get; set; }
    }
}
