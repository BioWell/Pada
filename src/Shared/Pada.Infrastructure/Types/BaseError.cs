using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public sealed class BaseError
    {
        public IDictionary<string, string[]> Messages { get; } = new Dictionary<string, string[]>();

        public BaseError(string code, string message)
        {
            Messages.Add(code, new[] {message});
        }

        public BaseError(string code, string[] messages)
        {
            Messages.Add(code, messages);
        }
        
        public BaseError(IDictionary<string, string[]> messages )
        {
            Messages = messages;
        }
    }
}