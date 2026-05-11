    using System;
    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models
    {
        public class AddDeviceRuntimeStatusRequest  
        {
    [Required]
        public int Id { get; set; }

        [Required]
        public string DeviceId { get; set; }

        [Required]
        public int BuildingId { get; set; }

        [Required]
        public string DeviceType { get; set; }

        [Required]
        public string CurrentStatus { get; set; }

        [Required]
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