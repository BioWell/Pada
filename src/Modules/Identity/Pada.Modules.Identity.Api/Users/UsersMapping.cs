using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Pada.Modules.Identity.Api.Users.Models.Requests;
using Pada.Modules.Identity.Application.Users.Features.PersonalInformation;
using Pada.Modules.Identity.Application.Users.Features.RegisterNewUser;

namespace Pada.Modules.Identity.Api.Users
{
    public class UsersMapping : Profile
    {
        public UsersMapping()
        {
            CreateMap<RegisterNewUserRequest, RegisterNewUserCommand>().ConstructUsing(req => new
                RegisterNewUserCommand(req.Id ?? Guid.NewGuid(), req.Email, req.FirstName, req.LastName, req.Name,
                    req.UserName, req.PhoneNumber, req.Password,
                    req.Permissions != null ? req.Permissions.ToList() : new List<string>(),
                    req.UserType, req.IsAdministrator, req.IsActive,
                    req.Roles != null ? req.Roles.ToList() : new List<string>(),
                    req.EmailConfirmed, req.PhotoUrl, req.Status));

            CreateMap<RegisterNewUserByPhoneRequest, RegisterNewUserByPhoneCommand>()
                .ForMember(d => d.PhoneNumber, 
                    opt => opt.MapFrom(s => s.Phone))
                .ConstructUsing(req => new RegisterNewUserByPhoneCommand(req.Phone));
            
            CreateMap<ChangePersonalInfoRequest, ChangePersonalInformationCommand>().ConstructUsing(req => new
                ChangePersonalInformationCommand(req.Id ?? Guid.NewGuid(), req.FirstName, req.LastName,
                    req.Name, req.PhoneNumber, req.PhotoUrl, req.Email));
        }
    }
}