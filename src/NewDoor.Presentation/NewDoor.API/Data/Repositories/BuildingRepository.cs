    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.Buildings.Models;
    using NewDoor.API.Infrastructure.Extensions;

    namespace NewDoor.API.Data.Repositories;

    public class BuildingRepository(DoWhattaProductDBContext context)
        : BaseRepository<Building>(context), IBuildingRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Building> buildings)
        {
            await DbSet.AddRangeAsync(buildings);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<(Building Building, List<Device> Devices)>> GetAllBuildingsWithDevicesAsync()
        {
            var buildings = await DbSet.ToListAsync();
            var devices = await _context.Set<Device>().ToListAsync();

            return buildings.Select(building => (
                Building: building,
                Devices: devices.Where(d => d.BuildingId == building.Id).ToList()
            )).ToList();
        }

        public async Task<List<(Building Building, List<Device> Devices)>> GetAllBuildingsWithDevicesAsync(BuildingFilterRequest filter)
        {
            IQueryable<Building> buildingQuery = DbSet;
            IQueryable<Device> deviceQuery = _context.Set<Device>();

            if (!string.IsNullOrWhiteSpace(filter.BuildingCode))
                buildingQuery = buildingQuery.ApplyStringFilter(nameof(Building.BuildingCode), filter.BuildingCode);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                buildingQuery = buildingQuery.ApplyStringFilter(nameof(Building.Name), filter.Name);

            if (!string.IsNullOrWhiteSpace(filter.Address))
                buildingQuery = buildingQuery.ApplyStringFilter(nameof(Building.Address), filter.Address);

            if (!string.IsNullOrWhiteSpace(filter.Status))
                buildingQuery = buildingQuery.Where(b => b.Status == filter.Status);

            if (filter.MinTotalDevices.HasValue || filter.MaxTotalDevices.HasValue)
                buildingQuery = buildingQuery.ApplyRangeFilter(nameof(Building.TotalDevices), filter.MinTotalDevices, filter.MaxTotalDevices);

            if (filter.MinOnlineDevices.HasValue || filter.MaxOnlineDevices.HasValue)
                buildingQuery = buildingQuery.ApplyRangeFilter(nameof(Building.OnlineDevices), filter.MinOnlineDevices, filter.MaxOnlineDevices);

            if (filter.MinOfflineDevices.HasValue || filter.MaxOfflineDevices.HasValue)
                buildingQuery = buildingQuery.ApplyRangeFilter(nameof(Building.OfflineDevices), filter.MinOfflineDevices, filter.MaxOfflineDevices);

            if (filter.MinActiveAlarms.HasValue || filter.MaxActiveAlarms.HasValue)
                buildingQuery = buildingQuery.ApplyRangeFilter(nameof(Building.ActiveAlarms), filter.MinActiveAlarms, filter.MaxActiveAlarms);

            if (filter.CreatedOnFrom.HasValue || filter.CreatedOnTo.HasValue)
                buildingQuery = buildingQuery.ApplyDateRangeFilter(nameof(Building.CreatedOn), filter.CreatedOnFrom, filter.CreatedOnTo);

            if (filter.UpdatedOnFrom.HasValue || filter.UpdatedOnTo.HasValue)
                buildingQuery = buildingQuery.ApplyDateRangeFilter(nameof(Building.UpdatedOn), filter.UpdatedOnFrom, filter.UpdatedOnTo);

            if (filter.Filters?.Any() == true)
            {
                foreach (var dynamicFilter in filter.Filters)
                {
                    buildingQuery = buildingQuery.ApplyFilter(dynamicFilter.FieldName!, dynamicFilter.Operator!, dynamicFilter.Value!);
                }
            }

            buildingQuery = buildingQuery.ApplySort(filter.SortBy, filter.SortDirection);
            buildingQuery = buildingQuery.ApplyPaging(filter.PageNumber, filter.PageSize);

            var buildings = await buildingQuery.ToListAsync();
            var buildingIds = buildings.Select(b => b.Id).ToList();

            deviceQuery = deviceQuery.Where(d => buildingIds.Contains(d.BuildingId));

            if (!string.IsNullOrWhiteSpace(filter.DeviceType))
                deviceQuery = deviceQuery.Where(d => d.DeviceType == filter.DeviceType);

            if (!string.IsNullOrWhiteSpace(filter.DeviceStatus))
                deviceQuery = deviceQuery.Where(d => d.Status == filter.DeviceStatus);

            if (!string.IsNullOrWhiteSpace(filter.Floor))
                deviceQuery = deviceQuery.ApplyStringFilter(nameof(Device.Floor), filter.Floor);

            if (!string.IsNullOrWhiteSpace(filter.Zone))
                deviceQuery = deviceQuery.ApplyStringFilter(nameof(Device.Zone), filter.Zone);

            var devices = await deviceQuery.ToListAsync();

            return buildings.Select(building => (
                Building: building,
                Devices: devices.Where(d => d.BuildingId == building.Id).ToList()
            )).ToList();
        }
    }