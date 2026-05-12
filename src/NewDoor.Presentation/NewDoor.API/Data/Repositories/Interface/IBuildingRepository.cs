    using DoWhatta.Platform.Data.Base;
    using DoWhatta.Platform.Core.DependencyInjection;
    using NewDoor.Platform.Entities;
    using NewDoor.Platform.DTO.Features.Buildings.Models;

    namespace NewDoor.API.Repositories.Interface;

    public interface IBuildingRepository : IBaseRepository<Building>, IscopedService
    {
        Task<int> AddRangeAsync(ICollection<Building> buildings);
        Task<List<(Building Building, List<Device> Devices)>> GetAllBuildingsWithDevicesAsync();
        Task<List<(Building Building, List<Device> Devices)>> GetAllBuildingsWithDevicesAsync(BuildingFilterRequest filter);
    }