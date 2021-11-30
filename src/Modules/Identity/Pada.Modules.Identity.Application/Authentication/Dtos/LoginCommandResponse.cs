using System.Collections.Generic;
using Pada.Infrastructure.Types;

namespace Pada.Modules.Identity.Application.Authentication.Dtos
{
    public class LoginCommandResponse : GatewayResponse<LoginResponse>
    {
        public LoginCommandResponse(LoginResponse data, bool isSuccess = true,
            IDictionary<string, string[]> errors = default) : base(data,
            isSuccess, errors)
        {
        }

        public LoginCommandResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }
    }
}