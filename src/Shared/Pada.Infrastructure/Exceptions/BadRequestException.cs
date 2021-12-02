using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message) { }
    }
}