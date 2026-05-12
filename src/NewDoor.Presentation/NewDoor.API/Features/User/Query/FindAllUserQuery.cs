using DoWhatta.Platform.Data.Mediator.Queries;
using DoWhatta.Platform.DTO.Features.User.Models;

namespace NewDoor.API.Features.User.Query
{
    public record FindAllUserQuery : BaseFindAllQuery<UserResponse>;
}
