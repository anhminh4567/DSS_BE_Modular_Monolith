using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Databases.Configurations.DiamondShapeConfig;
using FluentEmail.Core;
using FluentResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
            //DiamondShapeId shapeId = (DiamondShapeId)ids[0];
            DiamondCriteriaId criteriaId = (DiamondCriteriaId)ids[0];
            return _set.FirstOrDefaultAsync(d => d.CriteriaId == criteriaId);
        }
        public Task<DiamondPrice?> GetById(DiamondShapeId shapeId, DiamondCriteriaId criteriaId, CancellationToken cancellationToken = default)
        {
            return _set.Include(p => p.Criteria).ThenInclude(p => p.Shape)
                .FirstOrDefaultAsync(d => d.CriteriaId == criteriaId && d.Criteria.ShapeId == shapeId, cancellationToken);
        }
        public override async Task<List<DiamondPrice>> GetAll(CancellationToken token = default)
        {
            var getFromDb = await _dbContext.DiamondPrices.Include(p => p.Criteria).ToListAsync();
            return getFromDb;
        }
        public override async Task Create(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Create(entity, token);
            //RemoveAllKey(entity.ShapeId);
            RemoveAllCache();
        }
        public override async Task Update(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Update(entity, token);
            //RemoveAllKey(entity.ShapeId);
            RemoveAllCache();
        }
        public override async Task Delete(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Delete(entity, token);
            //RemoveAllKey(entity.ShapeId);
            RemoveAllCache();
        }

        public async Task CreateMany(List<DiamondPrice> prices)
        {
            await _set.AddRangeAsync(prices);
            RemoveAllCache();
        }

        public Task<List<DiamondPrice>> GetPriceByCriteria(DiamondCriteriaId diamondCriteriaId, bool? isLabDiamond = null, CancellationToken token = default)
        {
            if (isLabDiamond != null)
                return _set.Where(d => d.CriteriaId == diamondCriteriaId && d.IsLabDiamond == isLabDiamond && d.IsSideDiamond == false).Include(p => p.Criteria).ToListAsync();

            else
                return _set.Where(d => d.CriteriaId == diamondCriteriaId && d.IsSideDiamond == false).Include(p => p.Criteria).ToListAsync();
        }

        private void RemoveAllKey(DiamondShapeId diamondShapeId)
        {
            foreach (var cut in Enum.GetValues(typeof(Cut)))
            {
                string diamondKey = GetPriceKey(diamondShapeId, true, (Cut)cut);
                string diamondKeyNatural = GetPriceKey(diamondShapeId, false, (Cut)cut);
                _cache.Remove(diamondKey);
                _cache.Remove(diamondKeyNatural);
            }
            string sidediamondKey = GetSidePriceKey(diamondShapeId.Value, true);
            string sidediamondKeyNatural = GetSidePriceKey(diamondShapeId.Value, false);
            _cache.Remove(sidediamondKey);
            _cache.Remove(sidediamondKeyNatural);
        }
        //bool isFancyShape, 
        public Task<List<DiamondPrice>> GetSideDiamondPrice(bool isLabDiamond, CancellationToken token = default)
        {
            DiamondShape correctShape = null;
            //if (isFancyShape)
            //    correctShape = DiamondShape.FANCY_SHAPES;
            //else
            //    correctShape = DiamondShape.ROUND;
            correctShape = DiamondShape.ANY_SHAPES;
            //return _set.Where(d => d.ShapeId == correctShape.Id && d.IsSideDiamond == true && d.IsLabDiamond == isLabDiamond).Include(p => p.Criteria).ToListAsync();
            return _dbContext.DiamondCriteria
                .Where(d => d.ShapeId == correctShape.Id && d.IsSideDiamond == true)
                .Include(d => d.DiamondPrices)
                .SelectMany(d => d.DiamondPrices)
                .Where(dp => dp.IsLabDiamond == isLabDiamond)
                .Include(dp => dp.Criteria)
                .AsSplitQuery()
                .ToListAsync();
        }
        //bool isFancyShape,
        public async Task<List<DiamondPrice>> GetSideDiamondPriceByAverageCarat(bool isLabDiamond, float avgCarat, CancellationToken token = default)
        {
            DiamondShape correctShape = null;
            correctShape = DiamondShape.ANY_SHAPES;
            string diamondKey = GetSidePriceKey(correctShape.Id.Value, isLabDiamond);
            var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
            if (tryGet == null || tryGet.Count() == 0)
            {
                //var result = await _set.Where(d => d.ShapeId == correctShape.Id && d.IsLabDiamond == isLabDiamond && d.IsSideDiamond == true)
                //.Include(p => p.Criteria)
                //.Where(p => p.Criteria.CaratFrom <= avgCarat && p.Criteria.CaratTo >= avgCarat)
                //.ToListAsync();
                //_cache.Set(diamondKey, result);
                var result = await _dbContext.DiamondCriteria
                .Where(d => d.ShapeId == correctShape.Id && d.IsSideDiamond == true
                        && d.CaratFrom <= avgCarat && d.CaratTo >= avgCarat)
                        .Include(d => d.DiamondPrices)
                        .SelectMany(d => d.DiamondPrices)
                        .Where(dp => dp.IsLabDiamond == isLabDiamond)
                        .Include(dp => dp.Criteria)
                        .AsSplitQuery()
                        .ToListAsync();
                _cache.Set(diamondKey, result);
                return result;
            }
            else
                return tryGet;
        }

        public async Task<List<DiamondPrice>> GetPrice(Cut? cut, DiamondShape shape, bool isLabDiamond, CancellationToken token = default)
        {
            Cut? tobeComparedCut = cut;
            DiamondShape getShape = shape;
            bool isFancyShape = DiamondShape.IsFancyShape(getShape.Id);
            if (isFancyShape == false && tobeComparedCut == null)
                throw new Exception("round cut need to include cut");
            if (isFancyShape)
                tobeComparedCut = null;
            //if (isLabDiamond != null)
            //{
            string diamondKey = GetPriceKey(getShape.Id, isLabDiamond, tobeComparedCut);
            var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
            if (tryGet == null || tryGet.Count == 0)
            {
                //var get = await _set.Include(p => p.Criteria)
                //    .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == isLabDiamond && p.IsSideDiamond == false && p.Criteria.Cut == tobeComparedCut)
                //    .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                //_cache.Set(diamondKey, get);
                var get = await _dbContext.DiamondCriteria
                    .Where(d => d.ShapeId == getShape.Id && d.IsSideDiamond == false)
                    .Include(d => d.DiamondPrices)
                    .SelectMany(d => d.DiamondPrices)
                    .Where(dp => dp.IsLabDiamond == isLabDiamond && dp.Cut == tobeComparedCut)
                    .Include(dp => dp.Criteria)
                    .AsSplitQuery()
                    .ToListAsync();
                _cache.Set(diamondKey, get);
                return get;
            }
            return tryGet;
            //}
            //else
            //{
            //    string diamondKey = GetPriceKey(getShape.Id, true, tobeComparedCut);
            //    string diamondKeyNatural = GetPriceKey(getShape.Id, false, tobeComparedCut);
            //    List<DiamondPrice> results = new();
            //    var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
            //    var tryGetNatural = _cache.Get<List<DiamondPrice>>(diamondKeyNatural);
            //    if (tryGet is null || tryGet.Count == 0)
            //    {
            //        //var getFromDb = await _set.Include(p => p.Criteria)
            //        //  .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == true && p.IsSideDiamond == false && p.Criteria.Cut == tobeComparedCut)
            //        //  .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
            //        //_cache.Set(diamondKey, getFromDb);
            //        var getFromDb = await _set.Include(p => p.Criteria)
            //          .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == true && p.IsSideDiamond == false && p.Criteria.Cut == tobeComparedCut)
            //          .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
            //        _cache.Set(diamondKey, getFromDb);
            //        results.AddRange(getFromDb);
            //    }
            //    else
            //        results.AddRange(tryGet);
            //    if (tryGetNatural is null || tryGetNatural.Count == 0)
            //    {
            //        var getFromDb = await _set.Include(p => p.Criteria)
            //          .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == false && p.IsSideDiamond == false && p.Criteria.Cut == tobeComparedCut)
            //          .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
            //        _cache.Set(diamondKeyNatural, getFromDb);
            //        results.AddRange(getFromDb);
            //    }
            //    else
            //        results.AddRange(tryGetNatural);
            //    return results;
            //}
        }
        //dangerous function, make sure no field is null !!!
        public async Task<Result> DeleteMany(List<DiamondPriceId> priceIds,DiamondShape shape, bool Islab, bool IsSide, CancellationToken cancellationToken = default)
        {
            //var getCriteria = parameters.Select(x => x.CriteriaId).ToList();
            //var getShape = parameters.Select(x => x.DiamondShapeId).ToList();
            List<DiamondPrice> getResult = new List<DiamondPrice>();
            if (IsSide == false)
            {
                //getResult = await _set.IgnoreQueryFilters()
                //.Where(x => getCriteria.Contains(x.CriteriaId) && x.IsLabDiamond == Islab && x.IsSideDiamond == false && getShape.Contains(x.ShapeId))
                //.ToListAsync();
                getResult = await _dbContext.DiamondCriteria.Where(d => d.ShapeId == shape.Id && d.IsSideDiamond == false)
                    .Include(d => d.DiamondPrices)
                    .SelectMany(d => d.DiamondPrices)
                    .Where(dp => dp.IsLabDiamond == Islab)
                    .Include(dp => dp.Criteria)
                    .AsSplitQuery()
                    .ToListAsync();
            }
            else
            {
                //getResult = await _set.IgnoreQueryFilters()
                //.Where(x => getCriteria.Contains(x.CriteriaId) && x.IsLabDiamond == Islab && x.IsSideDiamond == true && getShape.Contains(x.ShapeId))
                //.ToListAsync();
                getResult = await _dbContext.DiamondCriteria.Where(d => d.ShapeId == shape.Id && d.IsSideDiamond == true)
                    .Include(d => d.DiamondPrices)
                    .SelectMany(d => d.DiamondPrices)
                    .Where(dp => dp.IsLabDiamond == Islab)
                    .Include(dp => dp.Criteria)
                    .AsSplitQuery()
                    .ToListAsync();
            }

            if (getResult == null || getResult.Count == 0)
                return Result.Fail("no diamond price found for this criteria");
            var getCorrectPrice = getResult.Where(x => priceIds.Contains(x.Id)); //getResult.Where(x => getShape.Any(s => s == x.ShapeId)).ToList();
            _set.RemoveRange(getCorrectPrice);
            //getShape.ForEach(x => RemoveAllKey(x));
            RemoveAllKey(shape.Id);
            return Result.Ok();
        }

        public async Task<List<DiamondPrice>> GetPriceIgnoreCache(DiamondShape shape, bool isLabDiamond, CancellationToken token = default)
        {
            DiamondShape getShape = shape;
            //if (isLabDiamond != null)
            //{
            //var get = await _set.Include(p => p.Criteria)
            //    .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == isLabDiamond && p.IsSideDiamond == false)
            //    .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
            var get = await _dbContext.DiamondCriteria
                .Where(d => d.ShapeId == getShape.Id && d.IsSideDiamond == false)
                .Include(d => d.DiamondPrices)
                .SelectMany(d => d.DiamondPrices)
                .Where(dp => dp.IsLabDiamond == isLabDiamond)
                .Include(dp => dp.Criteria)
                .AsSplitQuery()
                .ToListAsync();
            return get;
            //}
            //else
            //{
            //    List<DiamondPrice> results = new();
            //    var getFromDb = await _set.Include(p => p.Criteria)
            //      .Where(p => p.ShapeId == getShape.Id && p.IsLabDiamond == true && p.IsSideDiamond == false)
            //      .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
            //    results.AddRange(getFromDb);
            //    return results;
            //}
        }

        public void RemoveAllCache()
        {
            //var roundShape = DiamondShape.ROUND.Id;
            //var fancyShape = DiamondShape.FANCY_SHAPES.Id;
            //var anyShape = DiamondShape.ANY_SHAPES.Id;
            foreach (var shape in DiamondShape.All_Shape)
            {
                RemoveAllKey(shape.Id);
            }
            //RemoveAllKey(roundShape);
            //RemoveAllKey(fancyShape);
            //RemoveAllKey(anyShape);
        }
        private string GetPriceKey(DiamondShapeId shapeId, bool IsLabDiamond, Cut? cut)
        {
            bool isFancyShape = DiamondShape.IsFancyShape(shapeId);
            string key;
            if (isFancyShape)
                key = $"DP_{shapeId}";
            else
                key = $"DP_{shapeId}_{(int)cut}";
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
    }
}
