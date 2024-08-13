using DiamondShop.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common.ValueObjects
{
    public class Role
    {
        public Role(string roleId, string roleName)
        {
            Id = roleId;
            Name = roleName;
        }

        public string Id { get; init; }
        public string Name { get; init; }
        public List<RoleClaim> Claims { get; init; } = new();

        public override bool Equals(object? obj)
        {
            base.Equals(obj);
            if (obj == null ) 
                return false;
            if( obj is Role  is false)
            {
                return false;
            }
            var value = obj as Role;
            return Id == value.Id;
        }
    }
}
