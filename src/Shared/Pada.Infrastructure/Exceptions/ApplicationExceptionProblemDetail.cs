using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pada.Infrastructure.Exceptions
{
    public class ApplicationExceptionProblemDetail : ProblemDetails
    {
        public string Code { get; }

        public ApplicationExceptionProblemDetail(AppException exception)
        {
            Title = "Application rule broken";
            Status = StatusCodes.Status409Conflict;
            Detail = exception.Message;
            Type = "https://somedomain/application-rule-validation-error";
            Code = exception.Code;
        }
    }
}