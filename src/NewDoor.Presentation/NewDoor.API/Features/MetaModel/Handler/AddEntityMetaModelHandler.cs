using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.MetaModel.Command;
using DoWhatta.Platform.Data.Mediator.Handlers;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.Products.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.MetaModel.Handler
{
    public class AddEntityMetaModelHandler(IMapper mapper, IEntityMetaModelRepository EntityMetaModelRepository)
    : BaseAddHandler<AddEntityMetaModelCommand, AddEntityMetaModelRequest, EntityMetaModel, IEntityMetaModelRepository, EntityMetaModelResponse>(mapper,EntityMetaModelRepository);

}
