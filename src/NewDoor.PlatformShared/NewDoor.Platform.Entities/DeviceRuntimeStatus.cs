    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class DeviceRuntimeStatus : BaseEntity
        {
            [Key]
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