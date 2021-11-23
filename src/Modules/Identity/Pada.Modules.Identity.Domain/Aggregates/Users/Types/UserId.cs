﻿using System;
using Pada.Abstractions.Domain.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users.Types
{
    public class UserId : IdentityBase<Guid>
    {
        public UserId(Guid value) : base(value)
        {
        }
        
        public static implicit operator UserId(Guid id) => new(id);

        public static implicit operator Guid(UserId userId) => userId.Id;
    }
}