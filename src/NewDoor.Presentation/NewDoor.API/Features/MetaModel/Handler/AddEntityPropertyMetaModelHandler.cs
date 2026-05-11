using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.MetaModel.Command;
using DoWhatta.Platform.Data.Mediator.Handlers;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.MetaModel.Handler
{
  
    public class AddEntityPropertyMetaModelHandler(IMapper mapper, IEntityPropertyMetaModelRepository EntityPropertyMetaModelRepository)
   : BaseAddHandler<AddEntityPropertyMetaModelCommand, AddEntityPropertyMetaModelRequest, EntityPropertyMetaModel, IEntityPropertyMetaModelRepository, EntityPropertyMetaModelResponse>(mapper, EntityPropertyMetaModelRepository);
  
}
