using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Infrastructure.Utils;

namespace Pada.Infrastructure.Logging
{
    public class LoggingBehavior
    {
    }

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(IRequestHandler<TRequest, TResponse> handler, 
            ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public Task<TResponse> Handle(TRequest command, 
            CancellationToken cancellationToken, 
            RequestHandlerDelegate<TResponse> next)
        {
            var module = command.GetModuleName();
            var name = command.GetType().Name.Underscore();
            _logger.LogInformation("Handling a command: {Name} ({Module}) ...",
                name, module);
            return next();
        }
    }
}