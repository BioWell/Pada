using Pada.Infrastructure.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Exceptions
{
    public class UserNameAlreadyInUseException : AppException
    {
        public string Name { get; }

        public UserNameAlreadyInUseException(string name) : base($"UserName '{name}' is already in used.")
        {
            Name = name;
            Code = nameof(Name);
        }
    }
}