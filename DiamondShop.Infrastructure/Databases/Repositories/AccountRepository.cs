using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
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
            var find = await _set.Include(c => c.Roles).Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == accountId);
            return find;
        }
    }
}
