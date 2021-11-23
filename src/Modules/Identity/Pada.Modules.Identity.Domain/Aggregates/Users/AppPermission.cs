using System.Collections.Generic;
using Pada.Abstractions.Domain.Types;

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
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }

        public void Patch(AppPermission target)
        {
            target.Name = Name;
            target.GroupName = GroupName;
        }
    }
}