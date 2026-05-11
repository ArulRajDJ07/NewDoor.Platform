using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.MetaModel.Query;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.Products.Models;
using MediatR;

namespace NewDoor.API.Features.MetaModel.Handler
{
    public class FindEntityMetaModelByIdHandler(IMapper mapper, IEntityMetaModelRepository EntityMetaModelRepository) : IRequestHandler<FindEntityMetaModelByIdQuery, EntityMetaModelResponse>
    {
        public async Task<EntityMetaModelResponse> Handle(FindEntityMetaModelByIdQuery request, CancellationToken cancellationToken)
        {
            var Entity =
                await EntityMetaModelRepository.GetWithPropertiesAsync(request.Id);

            return mapper.Map<EntityMetaModelResponse>(Entity);
        }
    }
}
