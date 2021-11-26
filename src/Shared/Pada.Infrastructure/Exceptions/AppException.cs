namespace Pada.Infrastructure.Exceptions
{
    public class AppException : CustomException
    {
        public AppException(string message) : base(message) { }
    }
}