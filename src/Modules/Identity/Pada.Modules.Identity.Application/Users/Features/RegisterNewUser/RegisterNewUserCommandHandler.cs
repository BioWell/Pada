using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pada.Abstractions.Services.Mail;
using Pada.Abstractions.Services.Sms;
using Pada.Infrastructure.App;
using Pada.Infrastructure.Utils;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserCommandHandler : IRequestHandler<RegisterNewUserCommand>,
        IRequestHandler<VerifyEmailCommand>,
        IRequestHandler<RegisterNewUserByPhoneCommand>,
        IRequestHandler<SendVerificationEmailCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterNewUserCommandHandler> _logger;
        private readonly ISmsSender _smsSender;
        private readonly RegistrationOptions _registrationOptions;
        private readonly ICustomMailService _mailService;
        private readonly IAppIdentityDbContext _identityDbContext;
        private readonly AppOptions _appOptions;

        public RegisterNewUserCommandHandler(IUserRepository userRepository,
            ILogger<RegisterNewUserCommandHandler> logger,
            RegistrationOptions registrationOptions,
            ISmsSender smsSender,
            ICustomMailService mailService,
            IAppIdentityDbContext identityDbContext,
            IOptions<AppOptions> appOptions)
        {
            _userRepository = userRepository;
            _logger = logger;
            _registrationOptions = registrationOptions;
            _smsSender = smsSender;
            _mailService = mailService;
            _identityDbContext = identityDbContext;
            _appOptions = appOptions.Value;
        }

        public async Task<Unit> Handle(RegisterNewUserCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(RegisterNewUserCommand));

            if (!_registrationOptions.Enabled)
            {
                throw new IdentityException(string.Format("Disabled"));
            }

            var email = command.Email.ToLowerInvariant();
            var provider = email.Split("@").Last();
            if (_registrationOptions.InvalidEmailProviders?.Any(x => provider.Contains(x)) is true)
            {
                throw new EmailInvalidException(command.Email);
            }

            var user = await _userRepository.FindByEmailAsync(command.Email);
            if (user is { })
            {
                _logger.LogError($"Email '{command.Email}' already in used");
                throw new EmailAlreadyInUsedException(command.Email);
            }

            user = await _userRepository.FindByNameAsync(command.Name);
            if (user is { })
            {
                _logger.LogError($"Name '{command.Name}' already in used");
                throw new UserNameAlreadyInUseException(command.Name);
            }

            user = User.Create(command.Id,
                command.Email,
                command.FirstName,
                command.LastName,
                command.Name,
                command.UserName,
                command.PhoneNumber,
                command.Password,
                command.UserType,
                true,
                command.IsAdministrator,
                command.IsActive,
                command.EmailConfirmed,
                command.PhotoUrl,
                command.Status);

            user.ChangePermissions(command.Permissions?.Select(x => AppPermission.Of(x, "")).ToArray(), false);
            user.ChangeRoles(command.Roles?.Select(x => Role.Of(x, x)).ToArray(), false);

            await _identityDbContext.HandleTransactionAsync(beforeCommitHandler: async () =>
            {
                var result = await _userRepository.AddAsync(user);
                if (result.IsSuccess == false)
                    throw new RegisterNewUserFailedException(user.UserName);
                _logger.LogInformation("Created an account for the user with ID: '{Id}'.", user.Id);
            }, events: user.Events.ToList());

            return Unit.Value;
        }

        public async Task<Unit> Handle(RegisterNewUserByPhoneCommand command,
            CancellationToken cancellationToken = default)
        {
            var anyPhone = _userRepository.IsPhoneUsedAsync(command.PhoneNumber);
            if (anyPhone)
            {
                _logger.LogError($"PhoneNumber '{command.PhoneNumber}' already in used");
                throw new UserPhoneAlreadyInUseException(command.PhoneNumber);
            }

            var code = CodeGen.GenRandomNumber();
            var result = await _smsSender.SendCaptchaAsync(command.PhoneNumber, code);

            _logger.LogInformation($"RegisterNewUser '{result.Success}' message {result.Message}");
            return Unit.Value;
        }

        public async Task<Unit> Handle(SendVerificationEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(command));
            await EmailHelper.SendEmailVerification(command.UserId,
                _userRepository,
                _appOptions,
                _mailService);
            _logger.LogInformation("Verification email sent successfully for userId:{UserId}", command.UserId);

            //execute out of middleware transaction
            //this integration events will save in outbox before committing transaction and will execute in background after committing transaction
            // await _sqlDbContext.BeforeCommitHandler(async () =>
            // {
            //     //http://www.kamilgrzybek.com/design/how-to-publish-and-handle-domain-events/#comment-4602236620
            //     //Domain Event Notification often becomes an Integration Event which is send to Events Bus to other Bounded Context.
            //     //This event will save inner transaction in database and will execute as a background service out of transaction (outbox pattern)
            //
            //     //Events in application layer are integration events
            //     await _integrationEventDispatcher.DispatchAsync(
            //         new VerificationEmailSentIntegrationEvent(user.Id.Id, encodedCode, callbackUrl));
            // });

            return Unit.Value;
        }

        public async Task<Unit> Handle(VerifyEmailCommand command, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(command));

            var user = await _userRepository.FindByIdAsync(command.UserId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.Code));
            var result = await _userRepository.ConfirmEmailAsync(user.Id.ToString(), code);

            if (result.IsSuccess == false)
                throw new VerificationTokenIsInvalidException(user.Id.ToString());

            //this integration events will save in outbox after committing our TransactionalCommandMiddleware
            // await _integrationEventDispatcher.DispatchAsync(new EmailVerifiedIntegrationEvent(user.Id.ToString()));

            _logger.LogInformation("Email verified successfully for userId:{UserId}", user.Id);

            return Unit.Value;
        }
    }
}