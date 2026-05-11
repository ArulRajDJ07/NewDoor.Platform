using DoWhatta.Platform.Data.Mediator.BaseCommands;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using DoWhatta.Platform.DTO.Features.ProductFeatures.Models;
using DoWhatta.Platform.DTO.Features.ProductFeatures;

namespace NewDoor.API.Features.MetaModel.Command
{
    public record AddEntityMetaModelCommand(AddEntityMetaModelRequest EntityMetaModelRequest) :
    BaseAddCommand<AddEntityMetaModelRequest, EntityMetaModelResponse>(EntityMetaModelRequest);
}
