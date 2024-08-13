using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IUserIdentityRepository 
    {
        Task<IUserIdentity> GetById(string identityId);
        Task<IUserIdentity> GetByEmail(string email);
        Task<List<IUserIdentity>> GetByRole(Role id);
        

    }
}
