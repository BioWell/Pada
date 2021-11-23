using System;
using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public Guid UserId { get; }
        public string UserNameOrEmail { get; }

        public UserNotFoundException() : base("User not found.")
        {
        }
        public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
        {
            UserId = userId;
        }
        public UserNotFoundException(string userNameOrEmail) : base($"UserName or Email: '{userNameOrEmail}' was not found.")
        {
            UserNameOrEmail = userNameOrEmail;
        }
    }
}