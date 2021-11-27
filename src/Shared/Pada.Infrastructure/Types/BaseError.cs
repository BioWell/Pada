using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public class BaseError
    {
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

        public BaseError(string code, string error)
        {
            Errors.Add(code, new[] {error});
        }

        public BaseError(string code, string[] errors)
        {
            Errors.Add(code, errors);
        }
        
        public BaseError(IDictionary<string, string[]> errors )
        {
            Errors = errors;
        }
    }
}