using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Exceptions
{
    public class CoreException : CustomException
    {
        public CoreException(string message) : base(message) { }
    }
}