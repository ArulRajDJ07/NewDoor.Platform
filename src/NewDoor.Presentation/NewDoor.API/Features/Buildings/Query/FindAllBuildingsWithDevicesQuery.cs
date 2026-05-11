using MediatR;
using NewDoor.Platform.DTO.Features.Buildings.Models;

namespace NewDoor.API.Features.Buildings.Query
{
    public record FindAllBuildingsWithDevicesQuery(BuildingFilterRequest? Filter = null) : IRequest<List<BuildingWithDevicesResponse>>;
}
