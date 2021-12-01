using System.Collections.Generic;
using System.Linq;
using Pada.Abstractions.Domain.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users
{
    public class Role : ValueObject
    {
        public static Role Customer =>
            new(SecurityConstants.Role.Customer.Name, 
                SecurityConstants.Role.Customer.Name, 
                SecurityConstants.Role.Customer.Description,
                AppPermission.Users.Permissions);

        public static Role Admin =>
            new(SecurityConstants.Role.Admin.Name, 
                SecurityConstants.Role.Admin.Name, 
                SecurityConstants.Role.Admin.Description,
                AppPermission.Users.Permissions
                    .Union(AppPermission.Roles.Permissions)
                    .Union(AppPermission.Security.Permissions)
                    .ToArray());
        
        public string Name { get; }
        public string Code { get; }
        public string Description { get; }
        public IList<AppPermission> Permissions { get; }

        public Role(string name, string code, string description = null, params AppPermission[] permissions)
        {
            Name = name;
            Code = code;
            Permissions = permissions;
            Description = description;
        }
        
        public static Role Of(string name, string code, string description = null, params AppPermission[] permissions)
        {
            return new(name, code, description, permissions);
        }
        
        public static IEnumerable<Role> AllRoles()
        {
            return new[] { Customer, Admin}; 
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}