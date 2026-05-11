using AutoMapper;
using MediatR;
using NewDoor.API.Features.Devices.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.Devices.Handler
{
    public class BulkAddDeviceHandler(IMapper mapper, IDeviceRepository deviceRepository) 
        : IRequestHandler<BulkAddDeviceCommand, int>
    {
        public async Task<int> Handle(BulkAddDeviceCommand request, CancellationToken cancellationToken)
        {
            var devices = request.deviceRequest.deviceList.Select(d => mapper.Map<Device>(d)).ToList();
            return await deviceRepository.AddRangeAsync(devices);
        }
    }
}
