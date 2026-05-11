using AutoMapper;
using MediatR;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Devices.Models;
using NewDoor.API.Repositories.Interface;
using NewDoor.API.Features.Buildings.Query;

namespace NewDoor.API.Features.Buildings.Handler
{
    public class FindAllBuildingsWithDevicesHandler : IRequestHandler<FindAllBuildingsWithDevicesQuery, List<BuildingWithDevicesResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IBuildingRepository _buildingRepository;

        public FindAllBuildingsWithDevicesHandler(IMapper mapper, IBuildingRepository buildingRepository)
        {
            _mapper = mapper;
            _buildingRepository = buildingRepository;
        }

        public async Task<List<BuildingWithDevicesResponse>> Handle(FindAllBuildingsWithDevicesQuery request, CancellationToken cancellationToken)
        {
            List<(Platform.Entities.Building Building, List<Platform.Entities.Device> Devices)> buildingsWithDevices;

            if (request.Filter != null)
            {
                buildingsWithDevices = await _buildingRepository.GetAllBuildingsWithDevicesAsync(request.Filter);
            }
            else
            {
                buildingsWithDevices = await _buildingRepository.GetAllBuildingsWithDevicesAsync();
            }

            return buildingsWithDevices.Select(bd => new BuildingWithDevicesResponse
            {
                Id = bd.Building.Id,
                BuildingCode = bd.Building.BuildingCode,
                Name = bd.Building.Name,
                Address = bd.Building.Address,
                Status = bd.Building.Status,
                TotalDevices = bd.Building.TotalDevices,
                OnlineDevices = bd.Building.OnlineDevices,
                OfflineDevices = bd.Building.OfflineDevices,
                ActiveAlarms = bd.Building.ActiveAlarms,
                CreatedOn = bd.Building.CreatedOn,
                UpdatedOn = bd.Building.UpdatedOn,
                Devices = _mapper.Map<List<DeviceResponse>>(bd.Devices)
            }).ToList();
        }
    }
}
