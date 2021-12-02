using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Pada.Abstractions.Exceptions;

namespace Pada.Infrastructure.Validations
{
    public class AppValidationException : CustomException
    {
        public AppValidationException()
            : base("One or more validation failures have occurred.")
        {
            Code = "Validation";
            Errors = new Dictionary<string, string[]>();
        }

        public AppValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}