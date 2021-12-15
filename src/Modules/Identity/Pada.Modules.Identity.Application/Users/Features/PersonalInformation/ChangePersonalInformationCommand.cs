using System;
using MediatR;
using Pada.Modules.Identity.Domain.Aggregates.Users.Types;

namespace Pada.Modules.Identity.Application.Users.Features.PersonalInformation
{
    public class ChangePersonalInformationCommand : IRequest
    {
        public UserId UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public string PhoneNumber { get; }
        public string PhotoUrl { get; }
        public string Email { get; }
        public Guid Id { get; set; }

        public ChangePersonalInformationCommand(UserId userId, string firstName, string lastName, string name, string
            phoneNumber, string photoUrl, string email)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Name = name;
            PhoneNumber = phoneNumber;
            PhotoUrl = photoUrl;
            Email = email;
        }
    }
}