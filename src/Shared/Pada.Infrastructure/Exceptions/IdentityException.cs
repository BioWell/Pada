using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public class IdentityException : CustomException
    {
        public IdentityException(string message) : base(message) { }
    }
}