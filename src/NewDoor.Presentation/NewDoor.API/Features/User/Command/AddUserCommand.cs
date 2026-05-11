using DoWhatta.Platform.Data.Mediator.BaseCommands;
using DoWhatta.Platform.DTO.Features.Products.Models;
using DoWhatta.Platform.DTO.Features.User.Models;

namespace NewDoor.API.Features.User.Command
{
    public record AddUserCommand(AddUserRequest UserDetailRequest) : BaseAddCommand<AddUserRequest, UserResponse>(UserDetailRequest);
   
    public record UpdateUserCommand(AddUserRequest UserDetailRequest) : BaseUpdateCommand<AddUserRequest, UserResponse>(UserDetailRequest);
}
