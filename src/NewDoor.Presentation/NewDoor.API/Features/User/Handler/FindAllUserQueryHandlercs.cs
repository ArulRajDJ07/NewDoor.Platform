using AutoMapper;
using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.User.Query;
using DoWhatta.Platform.Data.Mediator.Handlers;
using DoWhatta.Platform.DTO.Features.User.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.User.Handler
{
    public class
      FindAllUserInfoQueryHandler(IMapper mapper, IUserRepository repository)
    : FindAllHandler<FindAllUserQuery, UserResponse, UserDetail, IUserRepository>(mapper, repository);
}
