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

        public RegisterNewUserCommandHandler(IUserRepository userRepository,
            ILogger<RegisterNewUserCommandHandler> logger,
            IAppIdentityDbContext identityDbContext)
        {
            _userRepository = userRepository;
            _logger = logger;
            _identityDbContext = identityDbContext;
        }

        public async Task<Unit> Handle(RegisterNewUserCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(RegisterNewUserCommand));
            var user = await _userRepository.FindByEmailAsync(command.Email);
            if (user is { })
            {
                _logger.LogError("Email '{Email}' already in used", command.Email);
                throw new EmailAlreadyInUsedException(command.Email);
            }
            
            user = await _userRepository.FindByNameAsync(command.Name);
            if (user is { })
            {
                _logger.LogError("UserName '{UserName}' already in used", command.UserName);
                throw new UserNameAlreadyInUseException(command.UserName);
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
                command.IsAdministrator,
                command.IsActive,
                command.EmailConfirmed,
                command.PhotoUrl,
                command.Status);
            
            user.ChangePermissions(command.Permissions?.Select(x => AppPermission.Of(x, "")).ToArray());
            user.ChangeRoles(command.Roles?.Select(x => Role.Of(x, x)).ToArray());
            
            var result = await _userRepository.AddAsync(user);
            if (result.IsSuccess == false)
                throw new RegisterNewUserFailedException(user.UserName);
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