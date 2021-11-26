using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class UpdateUserResponse : GatewayResponse<UserId>
    {
        public UpdateUserResponse(UserId data, bool isSuccess = true, IEnumerable<Error> errors = default)
            : base(data, isSuccess, errors)
        {
        }

        public UpdateUserResponse(IEnumerable<Error> errors) : base(errors)
        {
        }
    }
}