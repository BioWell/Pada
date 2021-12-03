using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pada.Abstractions.Services.Hangfire;
using Pada.Abstractions.Services.Mail;
using Pada.Infrastructure.App;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class UserEventHandler : INotificationHandler<NewUserRegisteredDomainEvent>
    {
        private readonly ILogger<UserEventHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICustomMailService _mailService;
        private readonly AppOptions _appOptions;
        private readonly IJobService _jobService;

        public UserEventHandler(ILogger<UserEventHandler> logger,
            IUserRepository userRepository,
            ICustomMailService mailService,
            IOptions<AppOptions> appOptions,
            IJobService jobService)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mailService = mailService;
            _jobService = jobService;
            _appOptions = appOptions.Value;
        }

        public async Task Handle(NewUserRegisteredDomainEvent notification,
            CancellationToken cancellationToken = default)
        {
            await EmailHelper.SendEmailVerification(notification.User.Id.ToString(),
                _userRepository,
                _appOptions,
                _mailService,
                _jobService);
            _logger.LogInformation("Verification email sent successfully for userId:{UserId}",
                notification.User.Id.ToString());
            _logger.LogInformation($"{nameof(NewUserRegisteredDomainEvent)} Raised.");
        }
    }
}