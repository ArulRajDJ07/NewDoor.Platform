    using AutoMapper;
using NewDoor.Platform.DTO.Features.Buildings.Models;
using NewDoor.Platform.DTO.Features.Buildings.Models;
    using NewDoor.Platform.Entities;

    namespace NewDoor.API.Features.Buildings.Mapper
    {
        public class BuildingMapper : Profile
        {
            public BuildingMapper()
            {
                CreateMap<AddBuildingRequest, Building>();
                CreateMap<Building, BuildingResponse>();
            }
        }
    }