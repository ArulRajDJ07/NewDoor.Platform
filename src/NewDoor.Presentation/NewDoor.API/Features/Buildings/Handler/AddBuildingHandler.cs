    using AutoMapper;
    using DoWhatta.Platform.Data.Mediator.Handlers;
    using NewDoor.Platform.DTO.Features.Buildings.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Buildings.Command;
using NewDoor.Platform.DTO.Features.Buildings.Models;

namespace NewDoor.API.Features.Buildings.Handler
    {
        public class AddBuildingHandler(IMapper mapper, IBuildingRepository buildingRepository)
            : BaseAddHandler<AddBuildingCommand, AddBuildingRequest, Building, IBuildingRepository, BuildingResponse>(mapper, buildingRepository);
    }