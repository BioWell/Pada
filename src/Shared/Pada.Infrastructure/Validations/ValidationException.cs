using System;
using System.Collections.Generic;

namespace Pada.Infrastructure.Validations
{
    public class AppValidationException : Exception
    {
        public AppValidationException(List<string> errors)
        {
            Errors = errors;
        }

        public List<string>  Errors { get; }
    }
}