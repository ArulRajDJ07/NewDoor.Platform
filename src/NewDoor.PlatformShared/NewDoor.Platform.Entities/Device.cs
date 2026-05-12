    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DoWhatta.Platform.Entities;
    using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Device : BaseEntity
        {
            [Key]
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