using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.MetaModel.Command;
using NewDoor.API.Features.MetaModel.Query;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.Entities;
using MediatR;
using Microsoft.Azure.Amqp.Framing;

namespace NewDoor.API.Features.MetaModel.Handler
{
    public class BulkEntityMasterHandler(IMapper mapper, IEntityPropertyMetaModelRepository EntityPropertyMetaModelRepository) : IRequestHandler<BulkAddEntityPropertyMetaModelCommand, int>
    {
       
        public async Task<int> Handle(BulkAddEntityPropertyMetaModelCommand request, CancellationToken cancellationToken)
        {
            var entities = request.bulkEntityPropertyMetaModelRequest.Properties.Select(p => new EntityPropertyMetaModel
            {
                EntityMetaModelId = p.EntityMetaModelId,
                PropertyName = p.PropertyName,
                PropertyType = p.PropertyType,
                IsRequired = p.IsRequired
            }).ToList();

            return await EntityPropertyMetaModelRepository.AddRangeAsync(entities);
        }
    }
}
