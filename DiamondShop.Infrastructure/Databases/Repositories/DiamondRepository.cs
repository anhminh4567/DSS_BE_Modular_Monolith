using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondRepository : BaseRepository<Diamond>, IDiamondRepository
    {
        public DiamondRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public void UpdateRange(List<Diamond> diamonds)
        {
            _set.UpdateRange(diamonds);
        }

        public override Task<Diamond?> GetById(params object[] ids)
        {
            DiamondId id = (DiamondId)ids[0];
            return _set.Include(d => d.DiamondShape).FirstOrDefaultAsync(d => d.Id == id);
        }

    }
}
