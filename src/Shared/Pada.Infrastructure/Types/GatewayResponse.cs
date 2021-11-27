﻿using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public class GatewayResponse<T> : ResultModel<T>
    {
        public GatewayResponse(T data, bool isSuccess = true, BaseError errors = default)
            : base(data, isSuccess, errors)
        {
        }

        public GatewayResponse(BaseError errors) : base(errors)
        {
        }

        public new static GatewayResponse<T> Create(T data, bool isSuccess = true, BaseError errors = default!)
        {
            return new(data, isSuccess, errors);
        }

        public new static GatewayResponse<T> Fail(BaseError errors = default!)
        {
            return new GatewayResponse<T>(default, false, errors);
        }

        public new static GatewayResponse<T> Success(T data)
        {
            return new GatewayResponse<T>(data);
        }
    }

    public class GatewayResponse : ResultModel
    {
        public GatewayResponse(bool success, BaseError errors = default!) : base(success, errors)
        {
        }

        public GatewayResponse(BaseError errors) : base(errors)
        {
        }

        public new static GatewayResponse Create(bool isSuccess = true, BaseError errors = default!)
        {
            return new(isSuccess, errors);
        }

        public new static GatewayResponse Fail(BaseError errors = default!)
        {
            return new GatewayResponse(false, errors);
        }

        public new static GatewayResponse Success()
        {
            return new GatewayResponse(true);
        }
    }
}