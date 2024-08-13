using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Common
{
    public interface IUserIdentity
    {
        public string IdentityId { get; }
        public List<Role> Roles { get;  }
        public List<Claim> Claims { get;  }
        public string Email { get;  }
    }
}
