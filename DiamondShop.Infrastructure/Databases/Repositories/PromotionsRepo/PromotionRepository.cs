using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo
{
    internal class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
    }
}
