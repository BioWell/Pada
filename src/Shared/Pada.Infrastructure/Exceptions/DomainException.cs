using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public abstract class DomainException : CustomException
    {
        public DomainException(string message) : base(message) { }
    }
}