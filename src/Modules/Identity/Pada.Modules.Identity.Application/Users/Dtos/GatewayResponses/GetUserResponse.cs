﻿using System.Collections.Generic;
using Pada.Infrastructure.Types;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses
{
    public class GetUserResponse : GatewayResponse<UserDto>
    {
        public GetUserResponse(UserDto data, bool isSuccess = true, IDictionary<string, string[]> errors = default) : base(data,
            isSuccess, errors)
        {
        }

        public GetUserResponse(IDictionary<string, string[]> errors) : base(errors)
        {
        }
    }
}