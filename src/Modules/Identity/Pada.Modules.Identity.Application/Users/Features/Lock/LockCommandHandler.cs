using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Features.Lock
{
    public class LockCommandHandler : IRequestHandler<LockUserCommand, LockUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LockCommandHandler> _logger;

        public LockCommandHandler(IUserRepository userRepository,
            ILogger<LockCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<LockUserResponse> Handle(LockUserCommand command, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(command, nameof(LockUserCommand));

            var user = await _userRepository.FindByIdAsync(command.UserId);
            if (user == null)
                throw new UserNotFoundException(command.UserId);

            var result = await _userRepository.LockUserAsync(user.Id.ToString());

            //this integration events will save in outbox before committing transaction and will execute in background after committing transaction
            // await _integrationEventDispatcher.DispatchAsync(new UserActivatedIntegrationEvent(user.Id));

            _logger.LogInformation($"user with id '{user.Id.ToString()}' locked.");

            return result;
        }
    }
}