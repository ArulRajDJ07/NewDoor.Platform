using DoWhatta.Platform.Data.Mediator.BaseCommands;
using DoWhatta.Platform.DTO.Features.MetaModel;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;

namespace NewDoor.API.Features.MetaModel.Command
{
    public record AddEntityPropertyMetaModelCommand(AddEntityPropertyMetaModelRequest EntityPropertyMetaModelRequest) 
        : BaseAddCommand<AddEntityPropertyMetaModelRequest, EntityPropertyMetaModelResponse>(EntityPropertyMetaModelRequest);

    public record BulkAddEntityPropertyMetaModelCommand(BulkAddEntityPropertyMetaModelRequest bulkEntityPropertyMetaModelRequest)
        : BaseAddCommand<BulkAddEntityPropertyMetaModelRequest, int>(bulkEntityPropertyMetaModelRequest);

}

