using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class CreateUserResponse : GatewayResponse<UserId>
    {
        public CreateUserResponse(UserId userId, bool isSuccess = true, IDictionary<string, string[]> errors = default)
            : base(userId, isSuccess, errors)
        {
        }

        public CreateUserResponse(IDictionary<string, string[]> errors = default) : base(errors)
        {
        }
        
        public CreateUserResponse(string code, string error) : base(code, error)
        {
        }
    }
}