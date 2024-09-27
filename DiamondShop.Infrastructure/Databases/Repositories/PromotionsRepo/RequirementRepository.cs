using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo
{
    internal class RequirementRepository : BaseRepository<PromoReq>, IRequirementRepository
    {
        public RequirementRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
    }
}
