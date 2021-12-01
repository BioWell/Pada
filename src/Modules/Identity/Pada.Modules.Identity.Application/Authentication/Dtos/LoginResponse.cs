using System.Collections.Generic;
using Pada.Infrastructure.Types;

namespace Pada.Modules.Identity.Application.Authentication.Dtos
{
    public class LoginResponse : GatewayResponse<LoginDto>
    {
        public LoginResponse(LoginDto data, bool isSuccess = true,
            IDictionary<string, string[]> errors = default) : base(data, isSuccess, errors)
        {
        }

        public LoginResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }

        public LoginResponse(string code, string error) : base(code, error)
        {
        }
    }
}