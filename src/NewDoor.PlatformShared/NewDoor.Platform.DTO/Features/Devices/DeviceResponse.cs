    using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.Devices.Models
    {
        public class DeviceResponse : BaseModel
        {
            public int Id { get; set; }

            public string DeviceId { get; set; }

            public string DeviceCode { get; set; } = string.Empty;

            public string DeviceName { get; set; }

            public string DeviceType { get; set; }

            public int BuildingId { get; set; }

            public string Floor { get; set; }

            public string Zone { get; set; }

            public string FirmwareVersion { get; set; }

            public string Status { get; set; }

            public DateTime CreatedOn { get; set; }

            public DateTime UpdatedOn { get; set; }
        }
    }