using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Pada.Abstractions.Domain.Types;
using Pada.Infrastructure.Utils;

namespace Pada.Modules.Identity.Domain.Aggregates.Users
{
    public sealed class AppPermission : ValueObject
    {
        private const char ScopeCharSeparator = '|';

        public string Name { get; private set; }
        public string GroupName { get; private set; }

        public AppPermission(string name, string groupName = null)
        {
            Name = name;
            GroupName = groupName;
        }

        public static AppPermission Of(string name, string groupName = null)
        {
            return new(name, groupName);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }

        public void Patch(AppPermission target)
        {
            target.Name = Name;
            target.GroupName = GroupName;
        }
        
        public static AppPermission TryCreateFromClaim(Claim claim)
        {
            AppPermission result = null!;
            if (claim.Type.EqualsInvariant("permission"))
            {
                result = new(claim.Value);
                if (result.Name.Contains(ScopeCharSeparator))
                {
                    var parts = claim.Value.Split(ScopeCharSeparator);
                    result.Name = parts.First();
                }
            }

            return result;
        }
    }
}