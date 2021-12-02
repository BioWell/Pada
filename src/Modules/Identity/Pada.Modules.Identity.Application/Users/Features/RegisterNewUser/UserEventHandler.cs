using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Domain.Aggregates.Users.DomainEvents;

namespace Pada.Modules.Identity.Application.Users.Features.RegisterNewUser
{
    public class UserEventHandler : INotificationHandler<NewUserRegisteredDomainEvent>
    {
        private readonly ILogger<UserEventHandler> _logger;

        public UserEventHandler(ILogger<UserEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(NewUserRegisteredDomainEvent notification, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"{nameof(NewUserRegisteredDomainEvent)} Raised.");
            return Task.CompletedTask;
        }
    }
}