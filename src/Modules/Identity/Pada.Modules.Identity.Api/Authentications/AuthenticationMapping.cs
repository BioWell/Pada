using AutoMapper;
using Pada.Modules.Identity.Api.Authentications.Models.Requests;
using Pada.Modules.Identity.Application.Authentication.Features.Login;
using Pada.Modules.Identity.Application.Authentication.Features.RefreshToken;
using Pada.Modules.Identity.Application.Authentication.Features.RevokeToken;

namespace Pada.Modules.Identity.Api.Authentications
{
    public class AuthenticationMapping : Profile
    {
        public AuthenticationMapping()
        {
            CreateMap<LoginRequest, LoginCommand>()
                .ConstructUsing(x => new LoginCommand(x.UserNameOrEmail, x.Password, x.RememberMe));

            CreateMap<RefreshTokenRequest, RefreshTokenCommand>()
                .ConstructUsing(x => new RefreshTokenCommand(x.AccessToken, x.RefreshToken));

            CreateMap<RevokeRefreshTokenRequest, RevokeRefreshTokenCommand>()
                .ConstructUsing(x => new RevokeRefreshTokenCommand(x.RefreshToken));
        }
    }
}