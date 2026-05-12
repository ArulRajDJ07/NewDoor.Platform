using NewDoor.Platform.DTO.Common;

namespace NewDoor.Platform.DTO.Features.Buildings.Models
{
    public class BuildingFilterRequest : FilterCriteria
    {
        public string? BuildingCode { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }
        public int? MinTotalDevices { get; set; }
        public int? MaxTotalDevices { get; set; }
        public int? MinOnlineDevices { get; set; }
        public int? MaxOnlineDevices { get; set; }
        public int? MinOfflineDevices { get; set; }
        public int? MaxOfflineDevices { get; set; }
        public int? MinActiveAlarms { get; set; }
        public int? MaxActiveAlarms { get; set; }
        public DateTime? CreatedOnFrom { get; set; }
        public DateTime? CreatedOnTo { get; set; }
        public DateTime? UpdatedOnFrom { get; set; }
        public DateTime? UpdatedOnTo { get; set; }
        
        public string? DeviceType { get; set; }
        public string? DeviceStatus { get; set; }
        public string? Floor { get; set; }
        public string? Zone { get; set; }
    }
}
