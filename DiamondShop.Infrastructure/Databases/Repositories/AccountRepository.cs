using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Account?> GetByIdentityId(string identityId, CancellationToken cancellationToken = default)
        {
            var find = await _set.Include(c => c.Roles).FirstOrDefaultAsync(c => c.IdentityId == identityId,cancellationToken);
            return find;
        }
        public override async Task<Account?> GetById(params object[] ids)
        {
            AccountId accountId = (AccountId)ids[0];
            if(accountId == Account.AnonymousCustomer.Id)
                return Account.AnonymousCustomer;
            var find = await _set.Where(c => c.Id == accountId).Include(c => c.Roles).Include(c => c.Addresses).FirstOrDefaultAsync();
            return find;
        }

        public Task<List<Account>> GetByRoles(List<AccountRole> roles, CancellationToken cancellationToken = default)
        {
            List<Account> result = new();
            var ids = roles.Select(x => x.Id).ToList();
            var roleQuery = _dbContext.AccountRoles.AsQueryable()
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Accounts)
                .SelectMany(x => x.Accounts).Distinct()
                    .Include(x => x.Roles);
            return roleQuery.ToListAsync();
        }

        public  Task<int> GetAccountCountsInRoles(List<AccountRole> roles, CancellationToken cancellationToken = default)
        {
            if (roles.Count == 0)
                return Task.FromResult(0);
            var ids = roles.Select(x => x.Id).ToList();
            var roleQuery = _dbContext.AccountRoles.AsQueryable()
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Accounts)
                .SelectMany(x => x.Accounts).Distinct().CountAsync();
            return roleQuery;
        }

        public Task<List<Account>> GetAccounts(List<AccountId> accountIds, CancellationToken cancellationToken = default)
        {
            return _set.Where(x => accountIds.Contains(x.Id)).ToListAsync();
        }
    }
}
