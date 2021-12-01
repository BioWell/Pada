using System;
using MediatR;

namespace Pada.Modules.Identity.Application.Authentication.Features.RevokeToken
{
    public class RevokeRefreshTokenCommand : IRequest
    {
        public string RefreshToken { get; }
        public Guid Id { get; set; } = new Guid();

        public RevokeRefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}