using System.Collections.Generic;
using Pada.Infrastructure.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos
{
    public class GenerateEmailConfirmationTokenResponse : GatewayResponse<string>
    {
        public string Token { get; }
        
        public GenerateEmailConfirmationTokenResponse(string data, bool isSuccess = true,
            IDictionary<string, string[]> errors = default) : base(data, isSuccess, errors)
        {
            Token = data;
        }

        public GenerateEmailConfirmationTokenResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }

        public GenerateEmailConfirmationTokenResponse(string code, string error) : base(code, error)
        {
        }
    }
}