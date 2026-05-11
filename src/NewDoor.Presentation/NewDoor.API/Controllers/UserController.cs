using NewDoor.API.Features.User.Command;
using NewDoor.API.Features.User.Query;
using NewDoor.API.Features.UserToken.Query;
using DoWhatta.Platform.DTO.Features.User.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;                 

namespace NewDoor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController(IMediator mediator) : ControllerBase
    {

        [HttpGet]
        public async Task<List<UserResponse>> GetAllUser()
        {
            var result = await mediator.Send(new FindAllUserQuery());
            return result;
        }

        [HttpPost("AcquireToken")]
        public async Task<TokenResponse> AcquireToken(TokenRequest tokenRequest)
        {
            return await mediator.Send(new TokenQuery(tokenRequest)); 
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] AddUserRequest userRequest)
        {
            return await mediator.Send(new AddUserCommand(userRequest));
        }

        [HttpPost("RefreshToken")]
        public async Task<TokenResponse> RefreshToken(TokenRequest tokenRequest)
        {
            return await mediator.Send(new TokenQuery(tokenRequest));
        }
    }
}
