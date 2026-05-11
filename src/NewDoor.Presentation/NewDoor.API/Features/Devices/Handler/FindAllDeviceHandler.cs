    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.Devices.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Devices.Query;

    namespace NewDoor.API.Features.Devices.Handler
    {
        public class FindAllDeviceHandler : IRequestHandler<FindAllDeviceQuery, List<DeviceResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IDeviceRepository _deviceRepository;

            public FindAllDeviceHandler(IMapper mapper, IDeviceRepository deviceRepository)
            {
                _mapper = mapper;
                _deviceRepository = deviceRepository;
            }

            public async Task<List<DeviceResponse>> Handle(FindAllDeviceQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new DeviceFilterRequest();
                var devices = await _deviceRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<DeviceResponse>>(devices);
            }
        }
    }