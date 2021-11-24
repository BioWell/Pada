using System;

namespace Pada.Infrastructure.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string Code { get; set; }
        protected DomainException(string message) : base(message)
        {
        }
    }
}