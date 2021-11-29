using System.Threading.Tasks;
using Pada.Abstractions.Services.Sms;

namespace Pada.Infrastructure.Services.Sms
{
    public class AliyunSmsSenderService: ISmsSender
    {
        public Task<bool> SendSmsAsync(SmsSend model)
        {
            throw new System.NotImplementedException();
        }

        public Task<(bool Success, string Message)> SendCaptchaAsync(string phone, string captcha)
        {
            throw new System.NotImplementedException();
        }
    }
}