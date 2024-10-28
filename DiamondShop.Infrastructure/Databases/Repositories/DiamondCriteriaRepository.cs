using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableCriteria(CancellationToken cancellationToken = default)
        {
            var result = await  _set
                .Where(x => x.IsSideDiamond == false)
                .GroupBy(x => new { x.CaratFrom, x.CaratTo })
                .Select(x => x.Key)
                .ToListAsync();
            return result.Select(result => (result.CaratFrom, result.CaratTo)).ToList();
        }

        public async Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableSideDiamondCriteria(CancellationToken cancellationToken = default)
        {
            var result = await _set
                .Where(x => x.IsSideDiamond == true)
                .GroupBy(x => new { x.CaratFrom, x.CaratTo })
                .Select(x => x.Key)
                .ToListAsync();
            return result.Select(result => (result.CaratFrom, result.CaratTo)).ToList();
        }
    }
}
