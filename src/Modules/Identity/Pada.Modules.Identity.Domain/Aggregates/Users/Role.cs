using System.Collections.Generic;
using Pada.Abstractions.Domain.Types;

namespace Pada.Modules.Identity.Domain.Aggregates.Users
{
    public class Role : ValueObject
    {
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
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}