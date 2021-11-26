using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class CreateUserResponse : GatewayResponse<UserId>
    {
        public CreateUserResponse(UserId userId, bool isSuccess = true, IEnumerable<Error> errors = default)
            : base(userId, isSuccess, errors)
        {
        }

        public CreateUserResponse(IEnumerable<Error> errors = default) : base(errors)
        {
        }
    }
}