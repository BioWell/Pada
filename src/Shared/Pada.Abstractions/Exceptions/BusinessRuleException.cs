using Pada.Abstractions.Domain;

namespace Pada.Abstractions.Exceptions
{
    public class BusinessRuleException : CustomException
    {
        public BusinessRuleException(IBusinessRule brokenRule)
            : base(brokenRule.Message)
        {
            Code = nameof(BrokenRule);
            BrokenRule = brokenRule;
            Details = brokenRule.Message;
        }

        public IBusinessRule BrokenRule { get; }

        public string Details { get; }

        public override string ToString()
        {
            return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
        }
    }
}