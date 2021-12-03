using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Pada.Abstractions.Services.Hangfire;
using Pada.Abstractions.Services.Mail;
using Pada.Infrastructure.App;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public static class EmailHelper
    {
        public static async Task SendEmailVerification(string userId,
            IUserRepository userRepository,
            AppOptions appOptions,
            ICustomMailService mailService,
            IJobService jobService)
        {
            var result = await userRepository.GenerateEmailConfirmationTokenAsync(userId);

            if (result.IsSuccess == false)
                throw new SendVerificationEmailFailedException(userId);

            var code = result.Token;

            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"{appOptions.ApiAddress}/api/v1/identity/users/{userId}/verify-email";
            callbackUrl = QueryHelpers.AddQueryString(callbackUrl, "code", encodedCode);

            string link = $"<a href='{callbackUrl}'>link</a>";
            string content =
                $"Welcome to Pada online learning application! Please verify your registration using this {link}.";

            var user = await userRepository.FindByIdAsync(userId);
            jobService.Enqueue(() => mailService.SendAsync(new CustomMailRequest(user.Email, "Verification Email", content)));
            // await mailService.SendAsync(new CustomMailRequest(user.Email, "Verification Email", content));
        }
    }
}