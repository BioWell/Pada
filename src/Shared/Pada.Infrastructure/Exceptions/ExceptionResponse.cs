using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Pada.Infrastructure.Types;

namespace Pada.Infrastructure.Exceptions
{
    public class ExceptionResponse : GatewayResponse<ProblemDetails>
    {
        public ExceptionResponse(ProblemDetails data, bool isSuccess = true, IEnumerable<Error> errors = default) :
            base(data, isSuccess, errors)
        {
        }

        public ExceptionResponse(IEnumerable<Error> errors) : base(errors)
        {
        }
    }
}