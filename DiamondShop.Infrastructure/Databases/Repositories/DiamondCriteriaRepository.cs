using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondCriteriaRepository : BaseRepository<DiamondCriteria>, IDiamondCriteriaRepository
    {
        public DiamondCriteriaRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task CreateMany(List<DiamondCriteria> diamondCriterias)
        {
            return _set.AddRangeAsync(diamondCriterias);

        }
    }
}
