using System.Collections.Generic;
using Pada.Infrastructure.Types;

namespace Pada.Modules.Identity.Application.Authentication.Dtos
{
    public class RefreshTokenResponse : GatewayResponse<RefreshTokenDto>
    {
        public RefreshTokenResponse(RefreshTokenDto data, bool isSuccess = true,
            IDictionary<string, string[]> errors = default) : base(data, isSuccess, errors)
        {
        }

        public RefreshTokenResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }

        public RefreshTokenResponse(string code, string error) : base(code, error)
        {
        }
    }
}