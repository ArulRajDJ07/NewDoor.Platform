    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Buildings.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Buildings.Query;
using NewDoor.Platform.DTO.Features.Buildings.Models;

namespace NewDoor.API.Features.Buildings.Handler
    {
        public class FindAllBuildingHandler(IMapper mapper, IBuildingRepository buildingRepository)
            : FindAllHandler<FindAllBuildingQuery, BuildingResponse, Building, IBuildingRepository>(mapper, buildingRepository);
    }