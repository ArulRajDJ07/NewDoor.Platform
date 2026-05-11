using NewDoor.Platform.DTO.Common;

namespace NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models
{
    public class DeviceRuntimeStatusFilterRequest : FilterCriteria
    {
        public string? DeviceId { get; set; }
        public int? BuildingId { get; set; }
        public string? DeviceType { get; set; }
        public string? CurrentStatus { get; set; }
        public bool? IsOnline { get; set; }
        public DateTime? LastHeartbeatFrom { get; set; }
        public DateTime? LastHeartbeatTo { get; set; }
        public DateTime? LastSeenFrom { get; set; }
        public DateTime? LastSeenTo { get; set; }
        public int? MinConsecutiveFailures { get; set; }
        public int? MaxConsecutiveFailures { get; set; }
        public decimal? MinTemperature { get; set; }
        public decimal? MaxTemperature { get; set; }
        public string? LastEventType { get; set; }
        public int? MinActiveAlarmCount { get; set; }
        public int? MaxActiveAlarmCount { get; set; }
        public string? SignalStrength { get; set; }
        public decimal? MinBatteryLevel { get; set; }
        public decimal? MaxBatteryLevel { get; set; }
        public DateTime? UpdatedOnFrom { get; set; }
        public DateTime? UpdatedOnTo { get; set; }
    }
}
