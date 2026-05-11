    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.DeviceRuntimeStatuss.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.DeviceRuntimeStatuss.Query;

    namespace NewDoor.API.Features.DeviceRuntimeStatuss.Handler
    {
        public class FindAllDeviceRuntimeStatusHandler : IRequestHandler<FindAllDeviceRuntimeStatusQuery, List<DeviceRuntimeStatusResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IDeviceRuntimeStatusRepository _deviceRuntimeStatusRepository;

            public FindAllDeviceRuntimeStatusHandler(IMapper mapper, IDeviceRuntimeStatusRepository deviceRuntimeStatusRepository)
            {
                _mapper = mapper;
                _deviceRuntimeStatusRepository = deviceRuntimeStatusRepository;
            }

            public async Task<List<DeviceRuntimeStatusResponse>> Handle(FindAllDeviceRuntimeStatusQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new DeviceRuntimeStatusFilterRequest();
                var deviceRuntimeStatuses = await _deviceRuntimeStatusRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<DeviceRuntimeStatusResponse>>(deviceRuntimeStatuses);
            }
        }
    }