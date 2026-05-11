using DoWhatta.Platform.DTO.Features.User.Models;
using MediatR;
namespace NewDoor.API.Features.UserToken.Query;

public class TokenQuery(TokenRequest Request) : IRequest<TokenResponse>
{
    public string Password => Request.Password;
    public string? Email => Request.Email;
    public long? PhoneNumber => Request.PhoneNumber;
}
