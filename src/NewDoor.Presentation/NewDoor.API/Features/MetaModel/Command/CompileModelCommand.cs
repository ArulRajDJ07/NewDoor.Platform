
using MediatR;
using DoWhatta.Platform.DTO.Features.MetaModel.Models;

namespace NewDoor.API.Features.MetaModel.Command;

public record CompileModelCommand(
    CompileMode Mode,
    DatabaseProvider Provider,
    int? EntityMetaModelId
) : IRequest<bool>;

