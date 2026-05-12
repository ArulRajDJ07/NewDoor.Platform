using AutoMapper;
using DoWhatta.Platform.DTO.Features.User.Models;
using DoWhatta.Platform.Entities;

namespace NewDoor.API.Features.User.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<AddUserRequest, UserDetail>();
            CreateMap<UserDetail, UserResponse>();
        }
    }
}