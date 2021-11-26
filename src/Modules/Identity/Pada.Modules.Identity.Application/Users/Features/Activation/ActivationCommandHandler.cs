using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Features.Activation
{
    public class ActivationCommandHandler : IRequestHandler<ActivateUserCommand, UpdateUserResponse>,
        IRequestHandler<DeActivateUserCommand, UpdateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ActivationCommandHandler> _logger;

        public ActivationCommandHandler(IUserRepository userRepository,
            ILogger<ActivationCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UpdateUserResponse> Handle(ActivateUserCommand command, CancellationToken cancellationToken)
        {
            Guard.Against.Null(command, nameof(ActivateUserCommand));

            var user = await _userRepository.FindByIdAsync(command.UserId);
            if (user == null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            user.ActivateUser();
            var response =  await _userRepository.UpdateAsync(user);

            _logger.LogInformation($"user with id '{user.Id}' activated successfully.");

            return  response;
        }

        public async Task<UpdateUserResponse> Handle(DeActivateUserCommand command, CancellationToken cancellationToken)
        {
            Guard.Against.Null(command, nameof(DeActivateUserCommand));

            var user = await _userRepository.FindByIdAsync(command.UserId);
            if (user == null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            user.DeactivateUser();
            var result =  await _userRepository.UpdateAsync(user);
            // if (result.IsSuccess == false)
            //     throw result.ToAppException();
            
            _logger.LogInformation($"user with id '{user.Id}' deactivated successfully.");

            return  result;
        }
    }
}