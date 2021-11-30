using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos
{
    public class UpdateUserResponse : GatewayResponse<UserId>
    {
        public UpdateUserResponse(UserId data, bool isSuccess = true, IDictionary<string, string[]> errors = default)
            : base(data, isSuccess, errors)
        {
        }

        public UpdateUserResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }
        
        public UpdateUserResponse(string code, string error) : base(code, error)
        {
        }
    }
}