using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pada.Abstractions.Domain.Types;
using Pada.Abstractions.Exceptions;
using Pada.Infrastructure.Types;
using Pada.Infrastructure.Validations;

namespace Pada.Infrastructure.Exceptions
{
    public class GlobalExceptionHandler : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                if (exception is not CustomException && exception.InnerException != null)
                {
                    while (exception.InnerException != null)
                    {
                        exception = exception.InnerException;
                    }
                }

                _logger.LogError(exception.Message);

                ResultModel<string> responseModel = null;

                BaseError error;
                
                switch (exception)
                {
                    case AppException e:
                        response.StatusCode = StatusCodes.Status409Conflict;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Application rule broken",
                            false,
                            error.Errors);
                        break;
                    case DomainException e:
                        response.StatusCode = StatusCodes.Status409Conflict;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Domain rule broken",
                            false,
                            error.Errors);
                        break;
                    case AppValidationException e:
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        responseModel = new ResultModel<string>("Input validation rules broken",
                            false,
                            e.Errors);
                        break;
                    case BadRequestException e:
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Bad request exception",
                            false,
                            error.Errors);
                        break;
                    case NotFoundException e:
                        response.StatusCode = StatusCodes.Status404NotFound;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Not found exception",
                            false,
                            error.Errors);
                        break;
                    case ApiException e:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Api services exception",
                            false,
                            error.Errors);
                        break;
                    case CoreException e:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Internal server exception",
                            false,
                            error.Errors);
                        break;
                    case IdentityAccessException e:
                        response.StatusCode = StatusCodes.Status403Forbidden;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Access exception",
                            false,
                            error.Errors);
                        break;
                    case IdentityException e:
                        response.StatusCode = StatusCodes.Status401Unauthorized;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("Authorization exception",
                            false,
                            error.Errors);
                        break;
                    case BusinessRuleException e:
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        error = new BaseError(e.Code, e.AppMessage);
                        responseModel = new ResultModel<string>("BusinessRule exception",
                            false,
                            error.Errors);
                        break;
                    default:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        error = new BaseError("System", "Internal server");
                        responseModel = new ResultModel<string>("Internal server exception",
                            false,
                            error.Errors);
                        break;
                }

                await response.WriteAsync(JsonConvert.SerializeObject(responseModel));
            }
        }
    }
}