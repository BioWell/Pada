﻿using System.Net;

namespace Pada.Abstractions.Exceptions
{
    public class ExceptionResponse
    {
        public ExceptionResponse(object response, HttpStatusCode statusCode)
        {
            Response = response;
            StatusCode = statusCode;
        }

        public object Response { get; }
        public HttpStatusCode StatusCode { get; }
    }
}