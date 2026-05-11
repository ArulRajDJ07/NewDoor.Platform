    using DoWhatta.Platform.Data.Base;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Data;
    using Microsoft.EntityFrameworkCore;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
    using NewDoor.API.Infrastructure.Extensions;

    namespace NewDoor.API.Data.Repositories;

    public class DeviceRuntimeStatusRepository(DoWhattaProductDBContext context)
        : BaseRepository<DeviceRuntimeStatus>(context), IDeviceRuntimeStatusRepository
    {
        private readonly DoWhattaProductDBContext _context = context;

        public async Task<int> AddRangeAsync(ICollection<DeviceRuntimeStatus> deviceRuntimeStatuses)
        {
            await DbSet.AddRangeAsync(deviceRuntimeStatuses);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<DeviceRuntimeStatus>> GetAllFilteredAsync(DeviceRuntimeStatusFilterRequest filter)
        {
            IQueryable<DeviceRuntimeStatus> query = DbSet;

            if (!string.IsNullOrWhiteSpace(filter.DeviceId))
                query = query.ApplyStringFilter(nameof(DeviceRuntimeStatus.DeviceId), filter.DeviceId);

            if (filter.BuildingId.HasValue)
                query = query.Where(d => d.BuildingId == filter.BuildingId.Value);

            if (!string.IsNullOrWhiteSpace(filter.DeviceType))
                query = query.Where(d => d.DeviceType == filter.DeviceType);

            if (!string.IsNullOrWhiteSpace(filter.CurrentStatus))
                query = query.Where(d => d.CurrentStatus == filter.CurrentStatus);

            if (filter.IsOnline.HasValue)
                query = query.Where(d => d.IsOnline == filter.IsOnline.Value);

            if (filter.LastHeartbeatFrom.HasValue || filter.LastHeartbeatTo.HasValue)
                query = query.ApplyDateRangeFilter(nameof(DeviceRuntimeStatus.LastHeartbeatUtc), filter.LastHeartbeatFrom, filter.LastHeartbeatTo);

            if (filter.LastSeenFrom.HasValue || filter.LastSeenTo.HasValue)
                query = query.ApplyDateRangeFilter(nameof(DeviceRuntimeStatus.LastSeenUtc), filter.LastSeenFrom, filter.LastSeenTo);

            if (filter.MinConsecutiveFailures.HasValue || filter.MaxConsecutiveFailures.HasValue)
                query = query.ApplyRangeFilter(nameof(DeviceRuntimeStatus.ConsecutiveFailures), filter.MinConsecutiveFailures, filter.MaxConsecutiveFailures);

            if (filter.MinTemperature.HasValue || filter.MaxTemperature.HasValue)
                query = query.ApplyRangeFilter(nameof(DeviceRuntimeStatus.CurrentTemperature), filter.MinTemperature, filter.MaxTemperature);

            if (!string.IsNullOrWhiteSpace(filter.LastEventType))
                query = query.Where(d => d.LastEventType == filter.LastEventType);

            if (filter.MinActiveAlarmCount.HasValue || filter.MaxActiveAlarmCount.HasValue)
                query = query.ApplyRangeFilter(nameof(DeviceRuntimeStatus.ActiveAlarmCount), filter.MinActiveAlarmCount, filter.MaxActiveAlarmCount);

            if (!string.IsNullOrWhiteSpace(filter.SignalStrength))
                query = query.Where(d => d.SignalStrength == filter.SignalStrength);

            if (filter.MinBatteryLevel.HasValue || filter.MaxBatteryLevel.HasValue)
                query = query.ApplyRangeFilter(nameof(DeviceRuntimeStatus.BatteryLevel), filter.MinBatteryLevel, filter.MaxBatteryLevel);

            if (filter.UpdatedOnFrom.HasValue || filter.UpdatedOnTo.HasValue)
                query = query.ApplyDateRangeFilter(nameof(DeviceRuntimeStatus.UpdatedOn), filter.UpdatedOnFrom, filter.UpdatedOnTo);

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