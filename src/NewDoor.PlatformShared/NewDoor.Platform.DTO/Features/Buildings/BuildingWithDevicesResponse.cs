using DoWhatta.Platform.DTO.Model;
using NewDoor.Platform.DTO.Features.Devices.Models;

namespace NewDoor.Platform.DTO.Features.Buildings.Models
{
    public class BuildingWithDevicesResponse : BaseModel
    {
        public int Id { get; set; }

        public string BuildingCode { get; set; }

        private string _buildingName = string.Empty;
        public string BuildingName 
        { 
            get => string.IsNullOrEmpty(_buildingName) ? Name : _buildingName;
            set => _buildingName = value;
        }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Status { get; set; }

        private bool _isActive;
        public bool IsActive 
        { 
            get => _isActive || (Status != null && Status.Equals("Active", StringComparison.OrdinalIgnoreCase));
            set => _isActive = value;
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int TotalDevices { get; set; }

        private int _deviceCount;
        public int DeviceCount 
        { 
            get => _deviceCount == 0 ? TotalDevices : _deviceCount;
            set => _deviceCount = value;
        }

        public int OnlineDevices { get; set; }

        public int OfflineDevices { get; set; }

        public int ActiveAlarms { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public List<DeviceResponse> Devices { get; set; } = new();
    }
}
