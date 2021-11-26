﻿using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class GetUserResponse : GatewayResponse<UserDto>
    {
        public GetUserResponse(UserDto data, bool isSuccess = true, IEnumerable<Error> errors = default) : base(data,
            isSuccess, errors)
        {
        }

        public GetUserResponse(IEnumerable<Error> errors) : base(errors)
        {
        }
    }
}