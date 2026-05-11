using AutoMapper;
using MediatR;
using NewDoor.API.Features.Buildings.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.Buildings.Handler
{
    public class BulkAddBuildingHandler(IMapper mapper, IBuildingRepository buildingRepository) 
        : IRequestHandler<BulkAddBuildingCommand, int>
    {
        public async Task<int> Handle(BulkAddBuildingCommand request, CancellationToken cancellationToken)
        {
            var buildings = request.buildingRequest.buildingList.Select(b => mapper.Map<Building>(b)).ToList();
            return await buildingRepository.AddRangeAsync(buildings);
        }
    }
}
