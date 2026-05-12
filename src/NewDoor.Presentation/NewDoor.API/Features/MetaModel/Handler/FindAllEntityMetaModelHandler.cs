using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.MetaModel.Query;
using DoWhatta.Platform.Data.Mediator.Handlers;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.ProductFeatures.Models;
using DoWhatta.Platform.DTO.Features.Products.Models;
using DoWhatta.Platform.Entities;
using MediatR;

namespace NewDoor.API.Features.MetaModel.Handler
{
   
    public class
    FindAllEntityMetaModelHandler(IMapper mapper, IEntityMetaModelRepository repository)
    : FindAllHandler<FindAllEntityMetaModelQuery, EntityMetaModelResponse, EntityMetaModel, IEntityMetaModelRepository>(mapper, repository);


}
