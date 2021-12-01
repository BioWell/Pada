using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Exceptions;
using Serilog;

namespace Pada.Infrastructure.Auth
{
    public static class Extensions
    {
        private const string SectionName = nameof(JwtOptions);

        public static IServiceCollection AddCustomJwtAuthentication(this IServiceCollection services,
            IConfiguration configuration,
            IList<ClaimPolicy> claimPolicies = null,
            IList<RolePolicy> rolePolicies = null,
            TokenStorageType storageType = TokenStorageType.InMemory,
            string sectionName = SectionName,
            Action<JwtBearerOptions> optionsFactory = null)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var options = configuration.GetSection(sectionName).Get<JwtOptions>();
            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddSingleton<IJwtTokenValidator, JwtTokenValidator>();

            if (storageType == TokenStorageType.InMemory)
                services.AddSingleton<IAccessTokenService, InMemoryAccessTokenService>();
            else
                services.AddSingleton<IAccessTokenService, DistributedTokenService>();

            services.AddScoped<AccessTokenValidatorMiddleware>();

            if (options.AuthenticationDisabled)
                services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                RequireAudience = options.RequireAudience,
                ValidIssuer = options.ValidIssuer,
                ValidIssuers = options.ValidIssuers,
                ValidateActor = options.ValidateActor,
                ValidAudience = options.ValidAudience,
                ValidAudiences = options.ValidAudiences,
                ValidateAudience = options.ValidateAudience,
                ValidateIssuer = options.ValidateIssuer,
                ValidateLifetime = options.ValidateLifetime,
                ValidateTokenReplay = options.ValidateTokenReplay,
                ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
                SaveSigninToken = options.SaveSigninToken,
                RequireExpirationTime = options.RequireExpirationTime,
                RequireSignedTokens = options.RequireSignedTokens,
                ClockSkew = TimeSpan.Zero
            };

            if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
                tokenValidationParameters.AuthenticationType = options.AuthenticationType;

            var hasCertificate = false;
            if (options.Certificate is { })
            {
                X509Certificate2 certificate = null;
                var password = options.Certificate.Password;
                var hasPassword = !string.IsNullOrWhiteSpace(password);
                if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
                {
                    certificate = hasPassword
                        ? new X509Certificate2(options.Certificate.Location, password)
                        : new X509Certificate2(options.Certificate.Location);
                    var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                    Log.Information(
                        "Loaded X.509 certificate from location: '{Location}' {KeyType}", options.Certificate.Location,
                        keyType);
                }

                if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
                {
                    var rawData = Convert.FromBase64String(options.Certificate.RawData);
                    certificate = hasPassword
                        ? new X509Certificate2(rawData, password)
                        : new X509Certificate2(rawData);
                    var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                    Log.Information("Loaded X.509 certificate from raw data {keyType}", keyType);
                }

                if (certificate is { })
                {
                    if (string.IsNullOrWhiteSpace(options.Algorithm)) options.Algorithm = SecurityAlgorithms.RsaSha256;

                    hasCertificate = true;
                    tokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
                    var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                    Log.Information("Using X.509 certificate for {ActionType} tokens", actionType);
                }
            }

            if (!string.IsNullOrWhiteSpace(options.IssuerSigningKey) && !hasCertificate)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm) || hasCertificate)
                    options.Algorithm = SecurityAlgorithms.HmacSha256;

                var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
                tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
                Log.Information("Using symmetric encryption for issuing tokens");
            }

            if (!string.IsNullOrWhiteSpace(options.NameClaimType))
                tokenValidationParameters.NameClaimType = options.NameClaimType;

            if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
                tokenValidationParameters.RoleClaimType = options.RoleClaimType;

            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication
            services
                .AddAuthentication(authOptions =>
                {
                    // will choose bellow JwtBearer handler for handling authentication because of our default schema to `JwtBearerDefaults.AuthenticationScheme` we could another schemas with their handlers
                    authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                    authOptions.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    //-- JwtBearerDefaults.AuthenticationScheme --
                    bearer.Authority = options.Authority;
                    bearer.Audience = options.Audience;
                    bearer.MetadataAddress = options.MetadataAddress;
                    bearer.SaveToken = options.SaveToken;
                    bearer.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                    bearer.RequireHttpsMetadata = options.RequireHttpsMetadata;
                    bearer.IncludeErrorDetails = options.IncludeErrorDetails;
                    bearer.TokenValidationParameters = tokenValidationParameters;
                    if (!string.IsNullOrWhiteSpace(options.Challenge)) bearer.Challenge = options.Challenge;

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                throw new IdentityException("The Token is expired.");
                            }

                            throw new IdentityException("An unhandled error has occurred.");
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                throw new IdentityException("You are not Authorized.");
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = _ =>
                            throw new IdentityAccessException("You are not authorized to access this resource."),
                    };
                    optionsFactory?.Invoke(bearer);
                });

            services.AddSingleton(options);
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthorization(authorizationOptions =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                authorizationOptions.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                foreach (var policy in claimPolicies)
                {
                    authorizationOptions.AddPolicy(policy.Name, x =>
                    {
                        x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        foreach (var policyClaim in policy.Claims)
                        {
                            x.RequireClaim(policyClaim.Type, policyClaim.Value);
                        }
                    });
                }

                foreach (var rolePolicy in rolePolicies)
                {
                    authorizationOptions.AddPolicy(rolePolicy.Name, x =>
                    {
                        x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        x.RequireRole(rolePolicy.Roles);
                    });
                }
            });

            return services;
        }

        public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AccessTokenValidatorMiddleware>();
        }
    }
}