using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class UpdateUserResponse : GatewayResponse<UserId>
    {
        public UpdateUserResponse(UserId data, bool isSuccess = true, BaseError errors = default)
            : base(data, isSuccess, errors)
        {
        }

        public UpdateUserResponse(BaseError errors) : base(errors)
        {
        }
    }
}