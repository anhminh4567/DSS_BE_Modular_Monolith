using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo
{
    internal class JewelryRepository : BaseRepository<Jewelry>, IJewelryRepository
    {
        public JewelryRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {

        }
        public override Task<Jewelry?> GetById(params object[] ids)
        {
            JewelryId id = (JewelryId)ids[0];
            return _set.Include(d => d.SideDiamonds).FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
