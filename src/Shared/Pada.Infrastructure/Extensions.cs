using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pada.Abstractions.Modules;
using Pada.Infrastructure.Exceptions;
using Pada.Infrastructure.Logging;
using Pada.Infrastructure.Web.Extensions;

namespace Pada.Infrastructure
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddModularInfrastructure(this WebApplicationBuilder builder,
            ConfigurationManager configuration,
            IList<IModule> modules)
        {
            var services = builder.Services;
            
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
            services.AddCoreExcepitons();

            // 3. Register Module services
            modules.ToList().ForEach(x =>
            {
                x.Init(configuration);
                x.Register(services);
            });
            
            builder.Host.UseLogging();
            
            return builder;
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