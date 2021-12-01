namespace Pada.Infrastructure.Exceptions
{
    public class IdentityAccessException: CustomException
    {
        public IdentityAccessException(string message) : base(message) { }
    }
}