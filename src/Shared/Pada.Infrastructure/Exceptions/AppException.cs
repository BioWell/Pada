using System;

namespace Pada.Infrastructure.Exceptions
{
    public class AppException : Exception
    {
        public virtual string Code { get; }

        public AppException(string message, string code = default!) : base(message)
        {
            Code = code;
        }
    }
}