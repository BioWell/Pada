using System;
using MediatR;
using Pada.Modules.Identity.Application.Authentication.Dtos;

namespace Pada.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string UserNameOrEmail { get; }
        public string Password { get; }
        public bool Remember { get; }
        public Guid Id { get; set; } = Guid.NewGuid();

        public LoginCommand(string userNameOrEmail, string password, bool remember)
        {
            UserNameOrEmail = userNameOrEmail;
            Password = password;
            Remember = remember;
        }
    }
}