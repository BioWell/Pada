using System;
using MediatR;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class VerifyEmailCommand : IRequest
    {
        public string UserId { get; }
        public string Code { get; }
        public Guid Id { get; set; }

        public VerifyEmailCommand(string userId, string code)
        {
            UserId = userId;
            Code = code;
        }
    }
}