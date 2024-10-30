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

        public async Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableCaratRange(CancellationToken cancellationToken = default)
        {
            var result = await _set
                .Where(x => x.IsSideDiamond == false)
                .GroupBy(x => new { x.CaratFrom, x.CaratTo })
                .Select(x => x.Key)
                .ToListAsync();
            return result.Select(result => (result.CaratFrom, result.CaratTo)).ToList();
        }
        public async Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableSideDiamondCaratRange(CancellationToken cancellationToken = default)
        {
            var result = await _set
                .Where(x => x.IsSideDiamond == true)
                .GroupBy(x => new { x.CaratFrom, x.CaratTo })
                .Select(x => x.Key)
                .ToListAsync();
            return result.Select(result => (result.CaratFrom, result.CaratTo)).ToList();
        }

        public async Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableCriteria(CancellationToken cancellationToken)
        {
            var result = await _set
            .Where(x => x.IsSideDiamond == false) // Filtering if necessary
            .GroupBy(x => new { x.CaratFrom, x.CaratTo }) // Group by CaratFrom
            .ToDictionaryAsync(
                group => (group.Key.CaratFrom, group.Key.CaratTo), // Key is the CaratFrom value
                group => group.ToList(), // Value is the list of DiamondCriteria with that CaratFrom
                cancellationToken);
            result.OrderBy(x => x.Key.CaratTo).ToDictionary(x => x.Key, x => x.Value);
            return result;
        }

        public async Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableSideDiamondCriteria(CancellationToken cancellationToken = default)
        {
            var result = await _set
           .Where(x => x.IsSideDiamond == true) // Filtering if necessary
           .GroupBy(x => new { x.CaratFrom, x.CaratTo }) // Group by CaratFrom
           .ToDictionaryAsync(
               group => (group.Key.CaratFrom, group.Key.CaratTo), // Key is the CaratFrom value
               group => group.ToList(), // Value is the list of DiamondCriteria with that CaratFrom
               cancellationToken);
            result.OrderBy(x => x.Key.CaratTo).ToDictionary(x => x.Key, x => x.Value);
            return result;
        }
    }
}
