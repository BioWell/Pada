using System.Collections.Generic;
using Pada.Infrastructure.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos
{
    public class CheckPasswordResponse: GatewayResponse<bool>
    {
        public bool IsPasswordValid { get; }
        
        public CheckPasswordResponse(bool isPasswordValid, bool isSuccess = true, IDictionary<string, string[]> errors = default)
            : base(isPasswordValid, isSuccess, errors)
        {
            IsPasswordValid = isPasswordValid;
        }

        public CheckPasswordResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }
        
        public CheckPasswordResponse(string code, string error) : base(code, error)
        {
        }
    }
}