using System.Linq;
using AutoMapper;
using Pada.Modules.Identity.Application.Users.Dtos;
using Pada.Modules.Identity.Domain.Aggregates.Users;

namespace Pada.Modules.Identity.Application.Users.Mapping
{
    public class UsersMapping : Profile
    {
        public UsersMapping()
        {
            CreateMap<User, UserDto>()
                .ForMember(des => des.Roles, conf => conf.MapFrom(s => s.Roles.Select(r => r.Name)))
                .ForMember(des => des.Permissions, conf => conf.MapFrom(s => s.Permissions.Select(p => p.Name)));
        }
    }
}