
using DoWhatta.Platform.DTO.Features.MetaModel.Models;
using MediatR;

public record GeneratePageFieldsCommand(
    EntityMetaModelResponse Entity
) : IRequest;
