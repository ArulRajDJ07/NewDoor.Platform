    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.Devices.Models;
    using NewDoor.API.Infrastructure.Extensions;

    namespace NewDoor.API.Data.Repositories;

    public class DeviceRepository(DoWhattaProductDBContext context)
        : BaseRepository<Device>(context), IDeviceRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<Device> devices)
        {
            await DbSet.AddRangeAsync(devices);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Device>> GetAllFilteredAsync(DeviceFilterRequest filter)
        {
            IQueryable<Device> query = DbSet;

            if (!string.IsNullOrWhiteSpace(filter.DeviceId))
                query = query.ApplyStringFilter(nameof(Device.DeviceId), filter.DeviceId);

            if (!string.IsNullOrWhiteSpace(filter.DeviceName))
                query = query.ApplyStringFilter(nameof(Device.DeviceName), filter.DeviceName);

            if (!string.IsNullOrWhiteSpace(filter.DeviceType))
                query = query.Where(d => d.DeviceType == filter.DeviceType);

            if (filter.BuildingId.HasValue)
                query = query.Where(d => d.BuildingId == filter.BuildingId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Floor))
                query = query.ApplyStringFilter(nameof(Device.Floor), filter.Floor);

            if (!string.IsNullOrWhiteSpace(filter.Zone))
                query = query.ApplyStringFilter(nameof(Device.Zone), filter.Zone);

            if (!string.IsNullOrWhiteSpace(filter.FirmwareVersion))
                query = query.ApplyStringFilter(nameof(Device.FirmwareVersion), filter.FirmwareVersion);

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(d => d.Status == filter.Status);

            if (filter.CreatedOnFrom.HasValue || filter.CreatedOnTo.HasValue)
                query = query.ApplyDateRangeFilter(nameof(Device.CreatedOn), filter.CreatedOnFrom, filter.CreatedOnTo);

            if (filter.UpdatedOnFrom.HasValue || filter.UpdatedOnTo.HasValue)
                query = query.ApplyDateRangeFilter(nameof(Device.UpdatedOn), filter.UpdatedOnFrom, filter.UpdatedOnTo);

            if (filter.Filters?.Any() == true)
            {
                foreach (var dynamicFilter in filter.Filters)
                {
                    query = query.ApplyFilter(dynamicFilter.FieldName!, dynamicFilter.Operator!, dynamicFilter.Value!);
                }
            }

            query = query.ApplySort(filter.SortBy, filter.SortDirection);
            query = query.ApplyPaging(filter.PageNumber, filter.PageSize);

            return await query.ToListAsync();
        }
    }