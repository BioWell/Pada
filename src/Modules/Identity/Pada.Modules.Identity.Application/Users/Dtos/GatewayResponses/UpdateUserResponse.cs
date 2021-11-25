using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class UpdateUserResponse : GatewayResponse<User>
    {
        public string UserId { get; }

        public UpdateUserResponse(User data, bool isSuccess = true, IEnumerable<Error> errors = default)
            : base(data, isSuccess, errors)
        {
            UserId = data.Id.Id.ToString();
        }

        public UpdateUserResponse(IEnumerable<Error> errors) : base(errors)
        {
        }
    }
}