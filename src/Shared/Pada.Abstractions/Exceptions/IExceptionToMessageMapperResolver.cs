using System;
using Pada.Abstractions.Messaging;

namespace Pada.Abstractions.Exceptions
{
    public interface IExceptionToMessageMapperResolver
    {
        IActionRejected Map<T>(T exception) where T : Exception;
    }
}