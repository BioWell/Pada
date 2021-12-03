using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Pada.Abstractions.Services.Mail;

namespace Pada.Infrastructure.Services.Mail
{
    public class SmtpMailService : ICustomMailService
    {
        private readonly MailOptions _config;
        private readonly ILogger<SmtpMailService> _logger;

        public SmtpMailService(IOptions<MailOptions> config,
            ILogger<SmtpMailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendAsync(CustomMailRequest request)
        {
            try
            {
                var email = new MimeMessage {Sender = MailboxAddress.Parse(_config.From)};
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                var builder = new BodyBuilder {HtmlBody = request.Body};
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.Host, _config.Port, SecureSocketOptions.SslOnConnect); //SslOnConnect SecureSocketOptions.StartTls
                await smtp.AuthenticateAsync(_config.UserName, _config.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                
                _logger.LogInformation(
                    "Email sent. From: {From}, To: {To}, Subject: {Subject}, Content: {Content}.",
                    _config.From,
                    request.To,
                    request.Subject,
                    request.Body);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}