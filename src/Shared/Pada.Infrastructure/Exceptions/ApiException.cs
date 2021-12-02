using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public class ApiException : CustomException
    {
        public ApiException(string message) : base(message) { }
    }
}