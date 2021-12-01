using System;
using Pada.Abstractions.Auth;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Authentication.Dtos
{
    public class LoginResponse
    {
        public LoginResponse(User user,AppJsonWebToken jwtToken, string refreshToken)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.UserName;
            JsonWebToken = jwtToken;
            RefreshToken = refreshToken;
        }
        
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        
        public AppJsonWebToken JsonWebToken { get; }
        public string RefreshToken { get; }
    }
}