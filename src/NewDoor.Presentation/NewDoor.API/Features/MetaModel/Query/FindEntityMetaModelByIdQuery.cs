using DoWhatta.Platform.Data.Mediator.Queries;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.ProductFeatures.Models;
using MediatR;

namespace NewDoor.API.Features.MetaModel.Query
{
    public record FindAllEntityMetaModelQuery : BaseFindAllQuery<EntityMetaModelResponse>;
    public record FindEntityMetaModelByIdQuery(long Id) :IRequest<EntityMetaModelResponse>;
}