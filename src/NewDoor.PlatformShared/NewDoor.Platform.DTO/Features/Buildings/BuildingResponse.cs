using DoWhatta.Platform.DTO.Model;
    namespace NewDoor.Platform.DTO.Features.Buildings.Models
    {
        public class BuildingResponse : BaseModel
        {
            public int Id { get; set; }


        public string BuildingCode { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Status { get; set; }

        public int TotalDevices { get; set; }

        public int OnlineDevices { get; set; }

        public int OfflineDevices { get; set; }

        public int ActiveAlarms { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
        }
    }