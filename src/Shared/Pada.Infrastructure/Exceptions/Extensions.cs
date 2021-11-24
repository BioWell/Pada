using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Pada.Infrastructure.Validations;

namespace Pada.Infrastructure.Exceptions
{
    public static class Extensions
    {
        public static IServiceCollection AddCoreExcepitons(this IServiceCollection services)
        {
            services.AddProblemDetails(x =>
            {
                // Control when an exception is included
                x.IncludeExceptionDetails = (ctx, _) =>
                {
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return false; //env.IsDevelopment() || env.IsStaging() || env.IsLocal();
                };

                x.Map<AppException>(ex => new ProblemDetails
                {
                    Title = "Application rule broken",
                    Status = StatusCodes.Status409Conflict,
                    Detail = ex.Message,
                    Type = "https://pada/application-rule-validation-error",
                });
                x.Map<DomainException>(ex => new ProblemDetails
                {
                    Title = "Domain rule broken",
                    Status = StatusCodes.Status409Conflict,
                    Detail = ex.Message,
                    Type = "https://somedomain/domain-rule-validation-error",
                });
                x.Map<AppValidationException>(ex => new ProblemDetails
                {
                    Title = "input validation rules broken",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = JsonConvert.SerializeObject(ex.Errors),
                    Type = "https://pada/input-validation-rules-error",
                });
                x.Map<BadRequestException>(ex => new ProblemDetails()
                {
                    Title = "bad request exception",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                    Type = "https://pada/bad-request-error",
                });
                x.Map<NotFoundException>(ex => new ProblemDetails()
                {
                    Title = "not found exception",
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message,
                    Type = "https://pada/not-found-error",
                });
                x.Map<ApiException>(ex => new ProblemDetails()
                {
                    Title = "api server exception",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message,
                    Type = "https://pada/api-server-error",
                });
                x.Map<CoreException>(ex => new ProblemDetails()
                {
                    Title = "internal server exception",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message,
                    Type = "https://pada/api-server-error",
                });
            });
            
            return services;
        }
    }
}