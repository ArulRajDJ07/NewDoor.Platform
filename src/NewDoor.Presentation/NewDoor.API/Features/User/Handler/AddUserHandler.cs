using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.User.Command;
using DoWhatta.Platform.Data.Mediator.Handlers;
using DoWhatta.Platform.DTO.Features.User.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.User.Handler
{
 
    public class AddUserHandler(IMapper mapper, IUserRepository UserRepository) : BaseAddHandler<AddUserCommand, AddUserRequest, UserDetail, IUserRepository, UserResponse>(mapper, UserRepository);

    public class UpdateUserHandler(IMapper mapper, IUserRepository UserRepository) : BaseUpdateHandler<UpdateUserCommand, AddUserRequest, UserDetail, IUserRepository, UserResponse>(mapper, UserRepository);
}
