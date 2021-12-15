using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Features.PersonalInformation
{
    public class ChangePersonalInformationCommandHandler : IRequestHandler<ChangePersonalInformationCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppIdentityDbContext _identityDbContext;
        private readonly ILogger<ChangePersonalInformationCommandHandler> _logger;

        public ChangePersonalInformationCommandHandler(IUserRepository userRepository,
            IAppIdentityDbContext identityDbContext, 
            ILogger<ChangePersonalInformationCommandHandler> logger)
        {
            _userRepository = userRepository;
            _identityDbContext = identityDbContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(ChangePersonalInformationCommand command,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(ChangePersonalInformationCommand));

            var user = await _userRepository.FindByIdAsync(command.UserId.Id.ToString());
            if (user == null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            await _identityDbContext.HandleTransactionAsync(beforeCommitHandler: async () =>
            {
                user.ChangePersonalInformation(command.FirstName, command.LastName, command.Name, command.Email,
                    command.PhoneNumber, command.PhotoUrl);

                var result = await _userRepository.UpdateAsync(user);

                if (result.IsSuccess == false)
                    throw new ChangeInfoException(command.UserId.ToString());

                _logger.LogInformation($"Personal information for userId '{user.Id}' changed successfully.");
            }, events: user.Events.ToList());

            return Unit.Value;
        }
    }
}