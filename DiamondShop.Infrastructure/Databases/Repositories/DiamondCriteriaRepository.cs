using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
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

        public async Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableCaratRange(DiamondShape diamondShape, Cut? cut, CancellationToken cancellationToken = default)
        {
            Cut? tobeComparedCut = cut;
            bool isfancyShape = diamondShape.IsFancy();
            if (isfancyShape)
                tobeComparedCut = null;
            //if ()
            //{
            //    var result = await _set
            //        .Where(x => x.IsSideDiamond == false && x.Cut == null)
            //        .GroupBy(x => new { x.CaratFrom, x.CaratTo })
            //        .Select(x => x.Key)
            //        .ToListAsync();
            //    return result.Select(result => (result.CaratFrom, result.CaratTo)).ToList();
            //}
            //else
            //{
            //    var result = await _set
            //        .Where(x => x.IsSideDiamond == false && x.Cut != null)
            //        .GroupBy(x => new { x.CaratFrom, x.CaratTo })
            //        .Select(x => x.Key)
            //        .ToListAsync();
            //    return result.Select(result => (result.CaratFrom, result.CaratTo)).ToList();
            //}

            var result = await _set.Where(x => x.IsSideDiamond == false && x.Cut == tobeComparedCut && x.ShapeId == diamondShape.Id)
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

        public async Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableCriteria(DiamondShape diamondShape, Cut? cut, CancellationToken cancellationToken)
        {
            Cut? tobeComparedCut = cut;
            bool isFancyShape = diamondShape.IsFancy();
            if (isFancyShape)
                tobeComparedCut = null;
            
            else
                if (tobeComparedCut == null)
                    throw new Exception("cut is required for round shape, only fancy shape need not provide cut");
            
            var result = await _set
               .Where(x => x.IsSideDiamond == false && x.Cut == tobeComparedCut && x.ShapeId == diamondShape.Id) // Filtering if necessary
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

        public Task<List<DiamondCriteria>> GetCriteriasByManyId(List<DiamondCriteriaId> diamondCriteriaIds, CancellationToken cancellationToken = default)
        {
            return _set
                .Where(x => diamondCriteriaIds.Contains(x.Id))
                .ToListAsync();
        }
    }
}
