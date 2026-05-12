    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    namespace NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models
    {
        public class BulkAddDeviceRuntimeStatusRequest  
        {
           public ICollection<AddDeviceRuntimeStatusRequest> deviceRuntimeStatusList { get; set; }
        }
    }