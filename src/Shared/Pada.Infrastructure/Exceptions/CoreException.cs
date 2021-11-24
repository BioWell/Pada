using System;
using System.Globalization;

namespace Pada.Infrastructure.Exceptions
{
    public class CoreException : ApplicationException
    {
        public CoreException() : base() { }

        public CoreException(string message) : base(message) { }

        public CoreException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}