using System;
using MediatR;
using Pada.Modules.Identity.Application.Authentication.Dtos;

namespace Pada.Modules.Identity.Application.Authentication.Features.RefreshToken
{
    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AccessToken { get; }
        public string RefreshToken { get; }

        public RefreshTokenCommand(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}