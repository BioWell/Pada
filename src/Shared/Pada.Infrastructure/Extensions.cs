using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ardalis.GuardClauses;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pada.Abstractions.Exceptions;
using Pada.Abstractions.Modules;
using Pada.Infrastructure.Exceptions;
using Pada.Infrastructure.Web.Extensions;
using Pada.Infrastructure.Validations;

namespace Pada.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddModularInfrastructure(this IServiceCollection services,
            ConfigurationManager configuration,
            IList<IModule> modules)
        {
            // 1. Controller MediatR & AutoMapper & Application layer
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            // 2. DbSet 
            //   Domain event & log
            // ...
            
            // 3. WebApi 
            services.AddWebApi(configuration);
            services.AddEndpointsApiExplorer();
            // services.AddSwaggerGen();
            
            // 4. Register Exceptions services
            services.AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>()
                .AddSingleton<IExceptionToMessageMapper, ExceptionToMessageMapper>()
                .AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>()
                .AddSingleton<IExceptionToMessageMapperResolver, ExceptionToMessageMapperResolver>();

            services.AddProblemDetails(x =>
            {
                // Control when an exception is included
                x.IncludeExceptionDetails = (ctx, _) =>
                {
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return false;//env.IsDevelopment() || env.IsStaging() || env.IsLocal();
                };

                x.Map<AppException>(ex => new ApplicationExceptionProblemDetail(ex));
                x.Map<AppValidationException>(ex => new ProblemDetails
                {
                    Title = "input validation rules broken",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = JsonConvert.SerializeObject(ex.ValidationResultModel.Errors),
                    Type = "https://somedomain/input-validation-rules-error",
                } );
                x.Map<BadRequestException>(ex => new ProblemDetails()
                {
                    Title = "bad request exception",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                    Type = "https://somedomain/bad-request-error",
                });
                x.Map<NotFoundException>(ex => new ProblemDetails()
                {
                    Title = "not found exception",
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message,
                    Type = "https://somedomain/not-found-error",
                });
                x.Map<ApiException>(ex => new ProblemDetails()
                {
                    Title = "api server exception",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message,
                    Type = "https://somedomain/api-server-error",
                });
            });

            // 3. Register Module services
            modules.ToList().ForEach(x =>
            {
                x.Init(configuration);
                x.Register(services);
            });
            
            return services;
        }

        public static WebApplication UseModularInfrastructure(this WebApplication app,
            IList<IModule> modules)
        {
            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }
            // app.UseHttpsRedirection();
            
            app.UseProblemDetails();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            foreach (var module in modules)
            {
                app.Logger.LogInformation($"Configuring the middleware for: '{module.Name} module'...");
                module.Use(app, app.Environment);
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers(); //.RequireAuthorization(); //enforce all controller tp authorize. we use AllowAnonymous attribute to bypass this rule
                endpoints.MapGet("/", context => context.Response.WriteAsync("Online Store API!"));
                foreach (var module in modules)
                {
                    app.Logger.LogInformation($"Configuring the endpoints for: '{module.Name} module', path: '/{module.Path}'...");
                    module.EndpointsConfigure(endpoints);
                }
            });
            
            return app;
        }
        
        public static bool IsLocal(this IHostEnvironment hostEnvironment) =>
            hostEnvironment?.IsEnvironment("local") ??
            throw new ArgumentNullException(nameof(hostEnvironment));
    }
}