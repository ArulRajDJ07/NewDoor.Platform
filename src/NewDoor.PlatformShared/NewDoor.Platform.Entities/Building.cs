    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
using DoWhatta.Platform.Entities;
using NewDoor.Platform.Entities;

    namespace NewDoor.Platform.Entities
    {
        public class Building : BaseEntity
        {
            [Key]
            public int Id { get; set; }



        [Required]
        public string BuildingCode { get; set; }

        [Required]
        public string Name { get; set; }


        public string Address { get; set; }

        [Required]
        public string Status { get; set; }


        public int TotalDevices { get; set; }


        public int OnlineDevices { get; set; }


        public int OfflineDevices { get; set; }


        public int ActiveAlarms { get; set; }


        public DateTime CreatedOn { get; set; }


        public DateTime UpdatedOn { get; set; }
        }
    }