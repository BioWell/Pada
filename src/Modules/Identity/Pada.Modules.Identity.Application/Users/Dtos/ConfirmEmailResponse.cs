using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Users.Dtos
{
    public class ConfirmEmailResponse : GatewayResponse<User>
    {
        public ConfirmEmailResponse(User data, bool isSuccess = true, IDictionary<string, string[]> errors = default) :
            base(data, isSuccess, errors)
        {
        }

        public ConfirmEmailResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }

        public ConfirmEmailResponse(string code, string error) : base(code, error)
        {
        }
    }
}