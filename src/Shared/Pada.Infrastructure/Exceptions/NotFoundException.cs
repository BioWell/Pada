using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message) { }
    }
}