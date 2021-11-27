using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class RegisterNewUserCommandHandler : IRequestHandler<RegisterNewUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterNewUserCommandHandler> _logger;
        private readonly IAppIdentityDbContext _identityDbContext;
        private readonly RegistrationOptions _registrationOptions;
        public RegisterNewUserCommandHandler(IUserRepository userRepository,
            ILogger<RegisterNewUserCommandHandler> logger,
            IAppIdentityDbContext identityDbContext, 
            RegistrationOptions registrationOptions)
        {
            _userRepository = userRepository;
            _logger = logger;
            _identityDbContext = identityDbContext;
            _registrationOptions = registrationOptions;
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
            
            user.ChangePermissions(command.Permissions?.Select(x => AppPermission.Of(x, "")).ToArray());
            user.ChangeRoles(command.Roles?.Select(x => Role.Of(x, x)).ToArray());

            var result = await _userRepository.AddAsync(user);
            if (result.IsSuccess == false)
                throw new RegisterNewUserFailedException(user.Name);
            _logger.LogInformation("Created an account for the user with ID: '{Id}'.", user.Id);
            
            //Option1: Using our transactional middleware for publishing domain events and integration events automatically
            //Option 2: Explicit calling domain events
            // await _userRepository.AddAsync(user);
            // await UnitOfWork.CommitAsync(); // will dispatch or domain events and domain event notifications
            
            // user.ChangePermissions(command.Permissions?.Select(x => Permission.Of(x, "")).ToArray());
            // user.ChangeRoles(command.Roles?.Select(x => Role.Of(x, x)).ToArray());

            return Unit.Value;
        }
    }
}