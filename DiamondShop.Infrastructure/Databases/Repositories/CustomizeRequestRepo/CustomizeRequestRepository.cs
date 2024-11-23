using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Repositories.CustomizeRequestRepo;
using Microsoft.EntityFrameworkCore;

namespace DiamondShop.Infrastructure.Databases.Repositories.CustomizeRequestRepo
{
    internal class CustomizeRequestRepository : BaseRepository<CustomizeRequest>, ICustomizeRequestRepository
    {
        public CustomizeRequestRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<CustomizeRequest?> GetById(params object[] ids)
        {
            var id = (CustomizeRequestId)ids[0];
            return await _set.Include(x => x.Account).Include(x => x.DiamondRequests).ThenInclude(x => x.Diamond).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CustomizeRequest?> GetDetail(CustomizeRequestId requestId, AccountId accountId)
        {
            var query = _set.AsQueryable();
            query = query.Include(p => p.Order);
            query = query.Include(p => p.JewelryModel.SizeMetals).ThenInclude(p => p.Metal);
            query = query.Include(p => p.SideDiamond);
            query = query.Include(p => p.DiamondRequests).ThenInclude(p => p.Diamond).ThenInclude(p => p.DiamondShape);
            query = query.Include(p => p.Order);
            query = query.Include(p => p.Jewelry);
            query = query.Include(p => p.Account);
            query = query.AsSplitQuery();
            return await query.FirstOrDefaultAsync(p => p.Id == requestId && p.AccountId == accountId);
        }
        public async Task<CustomizeRequest?> GetDetail(CustomizeRequestId requestId)
        {
            var query = _set.AsQueryable();
            query = query.Include(p => p.JewelryModel.SizeMetals).ThenInclude(p => p.Metal);
            query = query.Include(p => p.SideDiamond);
            query = query.Include(p => p.DiamondRequests).ThenInclude(p => p.Diamond).ThenInclude(p => p.DiamondShape);
            query = query.Include(p => p.Jewelry);
            query = query.Include(p => p.Order);
            query = query.Include(p => p.Account);
            query = query.AsSplitQuery();
            return await query.FirstOrDefaultAsync(p => p.Id == requestId);
        }
        public void UpdateRange(List<CustomizeRequest> customizeRequests)
        {
            _set.UpdateRange(customizeRequests);
        }

    }
}
