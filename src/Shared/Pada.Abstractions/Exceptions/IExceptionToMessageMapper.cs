using System;
using System.Collections.Generic;
using Pada.Abstractions.Messaging;

namespace Pada.Abstractions.Exceptions
{
    public interface IExceptionToMessageMapper
    {
        IEnumerable<Type> ExceptionTypes { get; }
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}