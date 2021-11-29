using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos.GatewayResponses;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserResponse>,
        IRequestHandler<GetUserByEmailQuery, GetUserResponse>,
        IRequestHandler<GetUserByUserNameQuery, GetUserResponse>,
        IRequestHandler<GetUserByPhoneQuery, GetUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper, ILogger<GetUserQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetUserResponse> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(query, nameof(GetUserByIdQuery));

            var user = await _userRepository.FindByIdAsync(query.UserId.ToString());
            if (user == null)
                throw new UserNotFoundException(query.UserId.ToString());

            _logger.LogInformation($"Get user with id '{user.Id}' successfully.");
            return new GetUserResponse(_mapper.Map<UserDto>(user));
        }

        public async Task<GetUserResponse> Handle(GetUserByEmailQuery query,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(query, nameof(GetUserByIdQuery));

            var user = await _userRepository.FindByEmailAsync(query.Email);
            if (user == null)
                throw new UserNotFoundException(query.Email);

            _logger.LogInformation($"Get user with Email '{user.Email}' successfully.");
            return new GetUserResponse(_mapper.Map<UserDto>(user));
        }

        public async Task<GetUserResponse> Handle(GetUserByUserNameQuery query,
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(query, nameof(GetUserByUserNameQuery));

            var user = await _userRepository.FindByNameAsync(query.UserName);
            if (user == null)
                throw new UserNotFoundException(query.UserName);

            _logger.LogInformation($"Get user with UserName '{user.UserName}' successfully.");
            return new GetUserResponse(_mapper.Map<UserDto>(user));
        }

        public async Task<GetUserResponse> Handle(GetUserByPhoneQuery query, 
            CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(query, nameof(GetUserByPhoneQuery));

            var user = await _userRepository.FindByPhoneAsync(query.Phone);
            if (user == null)
                throw new UserNotFoundException(query.Phone);

            _logger.LogInformation($"Get user with Phone '{user.PhoneNumber}' successfully.");
            return new GetUserResponse(_mapper.Map<UserDto>(user));
        }
    }
}