using AutoMapper;
using MediatR;
using NewDoor.API.Features.DeviceRuntimeStatuss.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.DeviceRuntimeStatuss.Handler
{
    public class BulkAddDeviceRuntimeStatusHandler(IMapper mapper, IDeviceRuntimeStatusRepository deviceRuntimeStatusRepository) 
        : IRequestHandler<BulkAddDeviceRuntimeStatusCommand, int>
    {
        public async Task<int> Handle(BulkAddDeviceRuntimeStatusCommand request, CancellationToken cancellationToken)
        {
            var deviceRuntimeStatuses = request.deviceRuntimeStatusRequest.deviceRuntimeStatusList.Select(d => mapper.Map<DeviceRuntimeStatus>(d)).ToList();
            return await deviceRuntimeStatusRepository.AddRangeAsync(deviceRuntimeStatuses);
        }
    }
}
