using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Databases.Configurations.DiamondShapeConfig;
using FluentEmail.Core;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondPriceRepository : BaseRepository<DiamondPrice>, IDiamondPriceRepository
    {
        private readonly IMemoryCache _cache;
        public DiamondPriceRepository(DiamondShopDbContext dbContext, IMemoryCache cache) : base(dbContext)
        {
            _cache = cache;
        }
        public override Task<DiamondPrice?> GetById(params object[] ids)
        {
            DiamondShapeId shapeId = (DiamondShapeId)ids[0];
            DiamondCriteriaId criteriaId = (DiamondCriteriaId)ids[1];
            return _set.FirstOrDefaultAsync(d => d.CriteriaId == criteriaId && d.ShapeId == shapeId);
        }
        public Task<DiamondPrice?> GetById(DiamondShapeId shapeId, DiamondCriteriaId criteriaId, CancellationToken cancellationToken = default)
        {
            return _set.Include(p => p.Criteria).Include(p => p.Shape).FirstOrDefaultAsync(d => d.CriteriaId == criteriaId && d.ShapeId == shapeId, cancellationToken);
        }
        public override async Task<List<DiamondPrice>> GetAll(CancellationToken token = default)
        {
            var getFromDb = await _dbContext.DiamondPrices.Include(p => p.Criteria).ToListAsync();
            return getFromDb;
        }
        public override async Task Create(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Create(entity, token);
            RemoveAllKey(entity.ShapeId);
        }
        public override async Task Update(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Update(entity, token);
            RemoveAllKey(entity.ShapeId);
        }
        public override async Task Delete(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Delete(entity, token);
            RemoveAllKey(entity.ShapeId);
        }

        public async Task<List<DiamondPrice>> GetPriceByShapes(DiamondShape shape , bool? isLabDiamond = null, CancellationToken token = default)
        {
            bool isFancyShape = DiamondShape.IsFancyShape(shape.Id);
            DiamondShape correctShape = shape;
            if(isFancyShape)
                correctShape = DiamondShape.FANCY_SHAPES;
            else
                correctShape = DiamondShape.ROUND;
            if (isLabDiamond != null)
            {
                string diamondKey = GetPriceKey(correctShape.Id.Value, isLabDiamond.Value);
                var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
                if (tryGet == null)
                {
                    var get = await _set.Include(p => p.Criteria)
                        .Where(p => p.ShapeId == correctShape.Id && p.IsLabDiamond == isLabDiamond && p.IsSideDiamond == false)
                        .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKey, get);
                    return get;
                }
                return tryGet;
            }
            else
            {
                string diamondKey = GetPriceKey(correctShape.Id.Value, true);
                string diamondKeyNatural = GetPriceKey(correctShape.Id.Value, false);
                List<DiamondPrice> results = new();
                var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
                var tryGetNatural = _cache.Get<List<DiamondPrice>>(diamondKeyNatural);
                if (tryGet is null || tryGet.Count == 0)
                {
                    var getFromDb = await _set.Include(p => p.Criteria)
                      .Where(p => p.ShapeId == correctShape.Id && p.IsLabDiamond == true && p.IsSideDiamond == false)
                      .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKey, getFromDb);
                    results.AddRange(getFromDb);
                }
                else
                    results.AddRange(tryGet);
                if (tryGetNatural is null || tryGetNatural.Count == 0)
                {
                    var getFromDb = await _set.Include(p => p.Criteria)
                      .Where(p => p.ShapeId == correctShape.Id && p.IsLabDiamond == false && p.IsSideDiamond == false)
                      .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKeyNatural, getFromDb);
                    results.AddRange(getFromDb);
                }
                else
                    results.AddRange(tryGetNatural);
                return results;
            }
        }

        public async Task CreateMany(List<DiamondPrice> prices)
        {
            await _set.AddRangeAsync(prices);
            var getShapeNames = DiamondShapeConfiguration.SHAPES.Select(x => x.Shape);
            getShapeNames.Select(x => $"DP_{x}")
                .ToArray()
                .ForEach(x => _cache.Remove(x));
        }

        public Task<List<DiamondPrice>> GetPriceByCriteria(DiamondCriteriaId diamondCriteriaId, bool? isLabDiamond = null, CancellationToken token = default)
        {
            if (isLabDiamond != null)
                return _set.Where(d => d.CriteriaId == diamondCriteriaId && d.IsLabDiamond == isLabDiamond && d.IsSideDiamond == false).Include(p => p.Criteria).ToListAsync();

            else
                return _set.Where(d => d.CriteriaId == diamondCriteriaId && d.IsSideDiamond == false).Include(p => p.Criteria).ToListAsync();
        }
        private string GetPriceKey(string shapeId, bool IsLabDiamond)
        {
            var key = $"DP_{shapeId}";
            if (IsLabDiamond)
                key += "_Lab";
            else
                key += "_Natural";
            return key;
        }
        private string GetSidePriceKey(string shapeId, bool IsLabDiamond)
        {
            var key = $"DP_Side_{shapeId}";
            if (IsLabDiamond)
                key += "_Lab";
            else
                key += "_Natural";
            return key;
        }
        private void RemoveAllKey(DiamondShapeId diamondShapeId)
        {
            string diamondKey = GetPriceKey(diamondShapeId.Value, true);
            string diamondKeyNatural = GetPriceKey(diamondShapeId.Value, false);
            string sidediamondKey = GetSidePriceKey(diamondShapeId.Value, true);
            string sidediamondKeyNatural = GetSidePriceKey(diamondShapeId.Value, false);
            _cache.Remove(diamondKey);
            _cache.Remove(diamondKeyNatural);
            _cache.Remove(sidediamondKey);
            _cache.Remove(sidediamondKeyNatural);
        }
        public async Task<List<DiamondPrice>> GetSideDiamondPriceByShape(DiamondShape shape, bool? islabDiamond = null, CancellationToken cancellationToken = default)
        {
            // now dont have to cache, because the side diamond is not that much
            if (islabDiamond != null)
            {
                var get = await _set.Include(p => p.Criteria)
                    .Where(p => p.ShapeId == shape.Id && p.IsLabDiamond == islabDiamond && p.IsSideDiamond == true)
                    .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                return get;
            }
            else
            {
                var getFromDb = await _set.Include(p => p.Criteria)
                  .Where(p => p.ShapeId == shape.Id && p.IsSideDiamond == true)
                  .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                return getFromDb;
            }
        }



        public Task<List<DiamondPrice>> GetSideDiamondPrice(bool? isLabDiamond = null, CancellationToken token = default)
        {
            if (isLabDiamond != null)
                return _set.Where(d => d.ShapeId == DiamondShape.ANY_SHAPES.Id && d.IsLabDiamond == isLabDiamond && d.IsSideDiamond == true).Include(p => p.Criteria).ToListAsync();
            else
                return _set.Where(d => d.ShapeId == DiamondShape.ANY_SHAPES.Id && d.IsSideDiamond == true).Include(p => p.Criteria).ToListAsync();

        }

        public Task<List<DiamondPrice>> GetSideDiamondPriceByAverageCarat(float avgCarat, bool? isLabDiamond = null, CancellationToken token = default)
        {
            if (isLabDiamond != null)
                    return _set.Where(d => d.ShapeId == DiamondShape.ANY_SHAPES.Id && d.IsLabDiamond == isLabDiamond && d.IsSideDiamond == true)
                    .Include(p => p.Criteria)
                    .Where(p => p.Criteria.CaratFrom <= avgCarat && p.Criteria.CaratTo >= avgCarat)
                    .ToListAsync();

            else

                return _set.Where(d => d.ShapeId == DiamondShape.ANY_SHAPES.Id && d.IsSideDiamond == true)
                        .Include(p => p.Criteria)
                        .Where(p => p.Criteria.CaratFrom <= avgCarat && p.Criteria.CaratTo >= avgCarat)
                        .ToListAsync();
        }

        public async Task<List<DiamondPrice>> GetPrice(bool isFancyShape, bool? isLabDiamond = null, CancellationToken token = default)
        {
            DiamondShape getShape;
            if (isFancyShape)
                getShape = _dbContext.DiamondShapes.IgnoreQueryFilters().ToList().First(s => s.Id == DiamondShape.FANCY_SHAPES.Id);
            else
                getShape = _dbContext.DiamondShapes.IgnoreQueryFilters().ToList().First(s => s.Id == DiamondShape.ROUND.Id);
            if (isLabDiamond != null)
            {
                string diamondKey = GetPriceKey(getShape.Id.Value, isLabDiamond.Value);
                var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
                if (tryGet == null)
                {
                    var get = await _set.Include(p => p.Criteria)
                        .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == isLabDiamond && p.IsSideDiamond == false)
                        .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKey, get);
                    return get;
                }
                return tryGet;
            }
            else
            {
                string diamondKey = GetPriceKey(getShape.Id.Value, true);
                string diamondKeyNatural = GetPriceKey(getShape.Id.Value, false);
                List<DiamondPrice> results = new();
                var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
                var tryGetNatural = _cache.Get<List<DiamondPrice>>(diamondKeyNatural);
                if (tryGet is null || tryGet.Count == 0)
                {
                    var getFromDb = await _set.Include(p => p.Criteria)
                      .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == true && p.IsSideDiamond == false)
                      .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKey, getFromDb);
                    results.AddRange(getFromDb);
                }
                else
                    results.AddRange(tryGet);
                if (tryGetNatural is null || tryGetNatural.Count == 0)
                {
                    var getFromDb = await _set.Include(p => p.Criteria)
                      .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == false && p.IsSideDiamond == false)
                      .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKeyNatural, getFromDb);
                    results.AddRange(getFromDb);
                }
                else
                    results.AddRange(tryGetNatural);
                return results;
            }
        }
        //dangerous function, make sure no field is null !!!
        public async Task<Result> DeleteMany(List<DeleteManyParameter> parameters, CancellationToken cancellationToken = default)
        {
            var getResult = await _set.IgnoreQueryFilters().Where(dp => parameters.Any(c =>
                dp.ShapeId == c.DiamondShapeId &&
                dp.CriteriaId == c.CriteriaId &&
                dp.IsLabDiamond == c.Islab &&
                dp.IsSideDiamond == c.IsSide))
                .ToListAsync();
            if (getResult == null || getResult.Count == 0)
                return Result.Fail("no diamond price found for this criteria");
            _set.RemoveRange(getResult);
            return Result.Ok();
        }
    }
}
