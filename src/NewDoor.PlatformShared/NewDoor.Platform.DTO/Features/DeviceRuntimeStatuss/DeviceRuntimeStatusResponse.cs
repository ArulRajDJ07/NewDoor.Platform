    using System;
    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models
    {
        public class DeviceRuntimeStatusResponse : BaseModel
        {
            public int Id { get; set; }

    public string DeviceId { get; set; }

        public int BuildingId { get; set; }

        public string DeviceType { get; set; }

        public string CurrentStatus { get; set; }

        public bool IsOnline { get; set; }

        public DateTime LastHeartbeatUtc { get; set; }

        public DateTime LastSeenUtc { get; set; }

        public int ConsecutiveFailures { get; set; }

        public decimal CurrentTemperature { get; set; }

        public string LastEventType { get; set; }

        public DateTime LastEventUtc { get; set; }

        public int ActiveAlarmCount { get; set; }

        public string SignalStrength { get; set; }

        public decimal BatteryLevel { get; set; }

        public DateTime StatusChangedUtc { get; set; }

        public DateTime UpdatedOn { get; set; }
        }
    }