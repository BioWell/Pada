using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos
{
    public class LockUserResponse : GatewayResponse<UserId>
    {
        public LockUserResponse(UserId data, bool isSuccess = true, IDictionary<string, string[]> errors = default) :
            base(data, isSuccess, errors)
        {
        }

        public LockUserResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }

        public LockUserResponse(string code, string error) : base(code, error)
        {
        }
    }
}