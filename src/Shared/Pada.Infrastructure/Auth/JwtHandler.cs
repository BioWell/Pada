using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Pada.Abstractions.Auth;
using Pada.Infrastructure.Utils;

namespace Pada.Infrastructure.Auth
{
    public sealed class JwtHandler : IJwtHandler
    {
        private static readonly ISet<string> DefaultClaims = new HashSet<string>
        {
            JwtRegisteredClaimNames.Sub,
            JwtRegisteredClaimNames.UniqueName,
            JwtRegisteredClaimNames.Jti,
            JwtRegisteredClaimNames.Iat,
            ClaimTypes.Role
        };

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
        private readonly JwtOptions _options;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILogger<JwtHandler> _logger;

        public JwtHandler(JwtOptions options,
            TokenValidationParameters tokenValidationParameters,
            ILogger<JwtHandler> logger)
        {
            _options = options;
            _tokenValidationParameters = tokenValidationParameters;
            _logger = logger;
        }

        public AppJsonWebToken GenerateJwtToken(
            string userName,
            string email,
            string userId,
            bool? isVerified = null,
            string firstName = null,
            string lastName = null,
            string phoneNumber = null,
            IList<Claim> usersClaims = null,
            IList<string> rolesClaims = null,
            IList<string> permissionsClaims = null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User ID claim (subject) cannot be empty.", nameof(userName));

            var now = DateTime.Now;
            string ipAddress = IpHelper.GetIpAddress();

            //https://leastprivilege.com/2017/11/15/missing-claims-in-the-asp-net-core-2-openid-connect-handler/
            //https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/a301921ff5904b2fe084c38e41c969f4b2166bcb/src/System.IdentityModel.Tokens.Jwt/ClaimTypeMapping.cs#L45-L125
            //https://stackoverflow.com/a/50012477/581476
            var jwtClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userName),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.UniqueName, userName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeMilliseconds().ToString()),
                new(JwtRegisteredClaimNames.NameId, userId),
                new(JwtRegisteredClaimNames.GivenName, firstName ?? string.Empty),
                new(JwtRegisteredClaimNames.FamilyName, lastName ?? string.Empty),
                new(ClaimTypes.MobilePhone, phoneNumber ?? string.Empty),
                new("ip", ipAddress)
            };

            if (rolesClaims?.Any() is true)
            {
                foreach (var role in rolesClaims)
                {
                    jwtClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (!string.IsNullOrWhiteSpace(_options.Audience))
            {
                jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _options.Audience));
            }

            if (permissionsClaims?.Any() is true)
            {
                foreach (var permissionsClaim in permissionsClaims)
                {
                    jwtClaims.Add(new Claim(CustomClaimTypes.Permission, permissionsClaim));
                }
            }

            if (usersClaims?.Any() is true)
            {
                jwtClaims = jwtClaims.Union(usersClaims).ToList();
            }

            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
            if (issuerSigningKey is null) throw new InvalidOperationException("Issuer signing key not set.");
            var signingCredentials =
                new SigningCredentials(issuerSigningKey, _options.Algorithm ?? SecurityAlgorithms.HmacSha256);

            var expire = now.AddMinutes(_options.ExpiryMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                notBefore: now,
                claims: jwtClaims,
                expires: expire,
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);


            return new AppJsonWebToken
            {
                IsVerified = isVerified,
                AccessToken = token,
                Expires = expire,
                UserId = userId,
                Email = email,
                Roles = rolesClaims?.ToList() ?? Enumerable.Empty<string>().ToList(),
                Permissions = permissionsClaims?.ToList() ?? Enumerable.Empty<string>().ToList()
            };
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal =
                    _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals
                    (_options.Algorithm ?? SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError("Token validation failed: {Message}", e.Message);
                return null;
            }
        }

        public AppRefreshToken GenerateRefreshToken(string userId, string ip = null)
        {
            var randomBytes = new byte[64];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            
            return new AppRefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiryOn = DateTime.Now.AddDays(_options?.RefreshTokenExpiryInDays ?? 7),
                CreatedOn = DateTime.Now,
                CreatedByIp = ip ?? IpHelper.GetIpAddress(),
                UserId = userId,
            };
        }

        public JsonWebTokenPayload GetTokenPayload(string accessToken)
        {
            _jwtSecurityTokenHandler.ValidateToken(accessToken, _tokenValidationParameters,
                out var validatedSecurityToken);
            if (!(validatedSecurityToken is JwtSecurityToken jwt)) return null;

            return new JsonWebTokenPayload
            {
                Subject = jwt.Subject,
                Role = jwt.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                Expires = jwt.ValidTo.ToUnixTimeMilliseconds(),
                Claims = jwt.Claims.Where(x => !DefaultClaims.Contains(x.Type))
                    .GroupBy(c => c.Type)
                    .ToDictionary(k => k.Key, v => v.Select(c => c.Value))
            };
        }
    }
}