namespace Pada.Infrastructure.Exceptions
{
    public class ApiException : CustomException
    {
        public ApiException(string message) : base(message) { }
    }
}