using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondPriceRepository : BaseRepository<DiamondPrice>, IDiamondPriceRepository
    {
        private readonly IMemoryCache _cache;
        //private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        //private static MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
        //    .SetPriority(CacheItemPriority.High)
       //.SetAbsoluteExpiration(TimeSpan.FromHours(1));
        private readonly IDiamondCriteriaRepository _diamondCriteriaRepository;
        public DiamondPriceRepository(DiamondShopDbContext dbContext, IMemoryCache cache , IDiamondCriteriaRepository diamondCriteriaRepository) : base(dbContext)
        {
            _cache = cache;
            _diamondCriteriaRepository = diamondCriteriaRepository;
        }
        public override Task<DiamondPrice?> GetById(params object[] ids)
        {
            //DiamondShapeId shapeId = (DiamondShapeId)ids[0];
            DiamondCriteriaId criteriaId = (DiamondCriteriaId)ids[0];
            return _set.FirstOrDefaultAsync(d => d.CriteriaId == criteriaId);
        }
        public int ExecuteUpdateCriteriaUpdateTime(DiamondCriteriaId[] criteriasId)
        {
            return _dbContext.DiamondCriteria.Where(x => criteriasId.Distinct().Contains(x.Id)).ExecuteUpdate(x => x.SetProperty(x => x.LastUpdatedTable, DateTime.UtcNow));
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
            //ExecuteUpdateCriteriaUpdateTime(new DiamondPrice[] { entity });
        }

        public override async Task Update(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Update(entity, token);
            //RemoveAllKey(entity.ShapeId);
            RemoveAllCache();
            //ExecuteUpdateCriteriaUpdateTime(new DiamondPrice[] { entity });
        }
        public override async Task Delete(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Delete(entity, token);
            //RemoveAllKey(entity.ShapeId);
            RemoveAllCache();
           // ExecuteUpdateCriteriaUpdateTime(new DiamondPrice[] { entity });
        }

        public async Task CreateMany(List<DiamondPrice> prices)
        {
            await _set.AddRangeAsync(prices);
            RemoveAllCache();
            ExecuteUpdateCriteriaUpdateTime(prices.Select(x => x.CriteriaId).ToArray());
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
                string diamondKey = GetPriceKey(diamondShapeId, true, (Cut)cut,null);
                string diamondKeyNatural = GetPriceKey(diamondShapeId, false, (Cut)cut, null);
                _cache.Remove(diamondKey);
                _cache.Remove(diamondKeyNatural);
            }
            string sidediamondKey = GetSidePriceKey(diamondShapeId.Value, true,null);
            string sidediamondKeyNatural = GetSidePriceKey(diamondShapeId.Value, false, null);
            _cache.Remove(sidediamondKey);
            _cache.Remove(sidediamondKeyNatural);
            
        }
        //bool isFancyShape, 
        public Task<List<DiamondPrice>> GetSideDiamondPrice(bool isLabDiamond, CancellationToken token = default)
        {
            DiamondShape correctShape = null;
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
            string diamondKey = GetSidePriceKey(correctShape.Id.Value, isLabDiamond,null);
            var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
            if (tryGet == null || tryGet.Count() == 0)
            {
                var result = await _dbContext.DiamondCriteria
                .Where(d => d.ShapeId == correctShape.Id && d.IsSideDiamond == true
                        && d.CaratFrom <= avgCarat && d.CaratTo >= avgCarat)
                        .Include(d => d.DiamondPrices)
                        .SelectMany(d => d.DiamondPrices)
                        .Where(dp => dp.IsLabDiamond == isLabDiamond)
                        .Include(dp => dp.Criteria)
                        .AsSplitQuery()
                        .ToListAsync();
                _cache.Set(diamondKey, result);//, options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token))
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
            string diamondKey = GetPriceKey(getShape.Id, isLabDiamond, tobeComparedCut,null);
            var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
            if (tryGet == null || tryGet.Count == 0)
            {
                var get = await _dbContext.DiamondCriteria
                    .Where(d => d.ShapeId == getShape.Id && d.IsSideDiamond == false)
                    .Include(d => d.DiamondPrices)
                    .SelectMany(d => d.DiamondPrices)
                    .Where(dp => dp.IsLabDiamond == isLabDiamond && dp.Cut == tobeComparedCut)
                    .Include(dp => dp.Criteria)
                    .AsSplitQuery()
                    .ToListAsync();
                _cache.Set(diamondKey, get);//, options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token))
                return get;
            }
            return tryGet;
        }
        //dangerous function, make sure no field is null !!!
        public async Task<Result> DeleteMany(List<DiamondPriceId> priceIds,DiamondShape shape, bool Islab, bool IsSide, CancellationToken cancellationToken = default)
        {
            //var getCriteria = parameters.Select(x => x.CriteriaId).ToList();
            //var getShape = parameters.Select(x => x.DiamondShapeId).ToList();
            List<DiamondPrice> getResult = new List<DiamondPrice>();
            if (IsSide == false)
            {
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
            ExecuteUpdateCriteriaUpdateTime(getCorrectPrice.Select(x => x.CriteriaId).ToArray());
            //getShape.ForEach(x => RemoveAllKey(x));
            RemoveAllCache();
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
        }

        public void RemoveAllCache()
        {
            if (_cache is MemoryCache concreteMemoryCache)
            {
                concreteMemoryCache.Clear();
            }
        }
        private string GetPriceKey(DiamondShapeId? shapeId, bool IsLabDiamond, Cut? cut, DiamondCriteriaId? criteriaId)
        {
            bool isFancyShape = DiamondShape.IsFancyShape(shapeId);
            string key;
            if(criteriaId == null)
            {
                if (isFancyShape)
                    key = $"DP_{shapeId.Value}";
                else
                    if (cut != null)
                        key = $"DP_{shapeId.Value}_{(int)cut}";
                    else
                        key = $"DP_{shapeId.Value}";
                if (IsLabDiamond)
                    key += "_Lab";
                else
                    key += "_Natural";
            }
            else
            {
                if (isFancyShape)
                    key = $"DP_{criteriaId.Value}";
                else
                    if(cut != null)
                        key = $"DP_{criteriaId.Value}_{(int)cut}";
                    else
                        key = $"DP_{criteriaId.Value}";
                if (IsLabDiamond)
                    key += "_Lab";
                else
                    key += "_Natural";
            }

            return key;
        }
        private string GetSidePriceKey(string? shapeId, bool IsLabDiamond, DiamondCriteriaId? criteriaId)
        {
            if(criteriaId == null)
            {
                ArgumentNullException.ThrowIfNull(shapeId);
                var key = $"DP_Side_{shapeId}";
                if (IsLabDiamond)
                    key += "_Lab";
                else
                    key += "_Natural";
                return key;
            }
            else
            {
                var key = $"DP_Side_{criteriaId.Value}";
                if (IsLabDiamond)
                    key += "_Lab";
                else
                    key += "_Natural";
                return key;
            }

        }
    }
}
