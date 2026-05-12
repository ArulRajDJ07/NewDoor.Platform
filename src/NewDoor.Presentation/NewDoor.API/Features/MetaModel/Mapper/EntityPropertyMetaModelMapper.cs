using AutoMapper;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.MetaModel.Mapper
{
    public class EntityPropertyMetaModelMapper : Profile
    {
        public EntityPropertyMetaModelMapper()
        {
            CreateMap<AddEntityPropertyMetaModelRequest, EntityPropertyMetaModel>();
            CreateMap<EntityPropertyMetaModel, EntityPropertyMetaModelResponse>();
        }
    }
}
