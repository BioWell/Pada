namespace Pada.Abstractions.Services.Mail
{
    public class CustomMailRequest
    {
        public CustomMailRequest(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }

        public string To { get; }
        public string Subject { get; }
        public string Body { get; }
    }
}