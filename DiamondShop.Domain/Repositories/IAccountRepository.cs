using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account?> GetByIdentityId(string identityId, CancellationToken cancellationToken = default);
        Task<List<Account>> GetByRoles(List<AccountRole> roles, CancellationToken cancellationToken = default);
        Task<int> GetAccountCountsInRoles(List<AccountRole> roles, CancellationToken cancellationToken = default);
        Task<List<Account>> GetAccounts(List<AccountId> accountIds, CancellationToken cancellationToken = default);
    }
}
