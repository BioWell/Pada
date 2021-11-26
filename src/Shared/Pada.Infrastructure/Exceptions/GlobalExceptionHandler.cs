using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

                switch (exception)
                {
                    case AppException e:
                        response.StatusCode = StatusCodes.Status409Conflict;
                        responseModel = new ResultModel<string>("Application rule broken",
                            false,
                            new List<Error>() {new Error(StatusCodes.Status409Conflict.ToString(), e.AppMessage)});
                        break;
                    case DomainException e:
                        response.StatusCode = StatusCodes.Status409Conflict;
                        responseModel = new ResultModel<string>("Domain rule broken", 
                            false,
                            new List<Error>() {new Error(StatusCodes.Status409Conflict.ToString(), e.AppMessage)});
                        break;
                    case AppValidationException e:
                        response.StatusCode = StatusCodes.Status400BadRequest;                        
                        responseModel = new ResultModel<string>("Input validation rules broken", false,
                            new List<Error>()
                            {
                                new Error(StatusCodes.Status409Conflict.ToString(), JsonConvert.SerializeObject(e.Errors))
                            });
                        break;
                    case BadRequestException e:
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        responseModel = new ResultModel<string>("Bad request exception", 
                            false,
                            new List<Error>() {new Error(StatusCodes.Status400BadRequest.ToString(), e.AppMessage)});
                        break;
                    case NotFoundException e:
                        response.StatusCode = StatusCodes.Status404NotFound;
                        responseModel = new ResultModel<string>("Not found exception", 
                            false,
                            new List<Error>() {new Error(StatusCodes.Status404NotFound.ToString(), e.AppMessage)});
                        break;
                    case ApiException e:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        responseModel = new ResultModel<string>("Api services exception", 
                            false,
                            new List<Error>() {new Error(StatusCodes.Status500InternalServerError.ToString(), e.AppMessage)});
                        break;
                    case CoreException e:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        responseModel = new ResultModel<string>("Internal server exception", 
                            false,
                            new List<Error>() {new Error(StatusCodes.Status500InternalServerError.ToString(), e.AppMessage)});
                        break;
                    default:
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        responseModel = new ResultModel<string>("Internal server exception", 
                            false,
                            new List<Error>() {new Error(StatusCodes.Status500InternalServerError.ToString(), "Internal error")});
                        break;
                }

                await response.WriteAsync(JsonConvert.SerializeObject(responseModel));
            }
        }
    }
}