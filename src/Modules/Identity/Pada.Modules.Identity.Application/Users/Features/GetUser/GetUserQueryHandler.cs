using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Pada.Modules.Identity.Application.Users.Contracts;
using Pada.Modules.Identity.Application.Users.Dtos.UseCaseResponses;
using Pada.Modules.Identity.Application.Users.Exceptions;

namespace Pada.Modules.Identity.Application.Users.Features.GetUser
{
    public class GetUserQueryHandler: IRequestHandler<GetUserByIdQuery, UserDto>,
        IRequestHandler<GetUserByEmailQuery, UserDto>,
        IRequestHandler<GetUserByUserNameQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(query, nameof(GetUserByIdQuery));

            var user = await _userRepository.FindByIdAsync(query.UserId.ToString());
            if (user == null)
                throw new UserNotFoundException(query.UserId.ToString());

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
        {
            Guard.Against.Null(query, nameof(GetUserByIdQuery));

            var user = await _userRepository.FindByEmailAsync(query.Email);
            if (user == null)
                throw new UserNotFoundException(query.Email);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> Handle(GetUserByUserNameQuery query, CancellationToken cancellationToken)
        {
            Guard.Against.Null(query, nameof(GetUserByUserNameQuery));
            
            var user = await _userRepository.FindByNameAsync(query.UserName);
            if (user == null)
                throw new UserNotFoundException(query.UserName);

            return _mapper.Map<UserDto>(user);
        }
    }
}