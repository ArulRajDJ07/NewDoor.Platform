using AutoMapper;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.Products.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.MetaModel.Mapper
{
    public class MetaModelMapper : Profile
    {
        public MetaModelMapper()
        {
            CreateMap<AddEntityMetaModelRequest, EntityMetaModel>();
            CreateMap<EntityMetaModel, EntityMetaModelResponse>();
        }
    }
}
