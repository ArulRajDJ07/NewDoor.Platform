using NewDoor.API.Data.Repositories.Interface;
using NewDoor.API.Features.UserToken.Query;
using DoWhatta.Platform.Core.Authentication.JWT;
using DoWhatta.Platform.DTO.Features.User.Models;
using MediatR;
using System.Security.Claims;

namespace NewDoor.API.Features.UserToken.Handler
{
    public class TokenQueryRequestHandler(IUserRepository userRepository,IJwtTokenService jwtTokenService) : IRequestHandler<TokenQuery, TokenResponse>
    {
        private readonly IUserRepository userRepository = userRepository;
        private readonly IJwtTokenService jwtTokenService = jwtTokenService;

        public async Task<TokenResponse> Handle(TokenQuery request, CancellationToken cancellationToken)
        {
            var userDetail = await userRepository.FindUserByEmailOrPhoneNumber(request.Password, request.Email, request.PhoneNumber);
            if(userDetail!=null)
            {
                List<Claim> claims = [];
                if (!string.IsNullOrWhiteSpace(request.Email))
                    claims.Add(new(ClaimTypes.Email, request.Email));
                if (request.PhoneNumber.HasValue)
                    claims.Add(new(ClaimTypes.MobilePhone, request.PhoneNumber.Value.ToString()));
                claims.Add(new(ClaimTypes.NameIdentifier, userDetail.Id.ToString()));
                claims.Add(new(ClaimTypes.Role, userDetail.RoleId.ToString()));

                var token = jwtTokenService.CreateTokenString(claims);
                var refreshToken = jwtTokenService.GenerateRefreshToken();
                //userDetail.RefreshToken = refreshToken;
                //userDetail.RefreshTokenExpireTime = DateTime.UtcNow.AddMinutes(_jwtBearerSettings.RefreshTokenValidityInMinutes);
                //await userRepository.UpdateAsync(userDetail);
                return new TokenResponse
                {
                    Token = token,
                    RefreshToken = refreshToken
                };
            }
            else
            {
                throw new BadHttpRequestException("Invalid username and Password provided");
            }
        }
    }
}
