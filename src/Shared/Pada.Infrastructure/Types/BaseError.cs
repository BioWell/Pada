using System.Collections.Generic;

namespace Pada.Infrastructure.Types
{
    public class BaseError
    {
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

        public string Code { get; set; } = "pada";

        public BaseError(string code, string error)
        {
            Code = code;
            Errors.Add(Code, new[] {error});
        }

        public BaseError(string code, string[] errors)
        {
            Code = code;
            Errors.Add(Code, errors);
        }

        public BaseError(IDictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
}