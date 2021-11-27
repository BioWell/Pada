using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class CreateUserResponse : GatewayResponse<UserId>
    {
        public CreateUserResponse(UserId userId, bool isSuccess = true, BaseError errors = default)
            : base(userId, isSuccess, errors)
        {
        }

        public CreateUserResponse(BaseError errors = default) : base(errors)
        {
        }
    }
}