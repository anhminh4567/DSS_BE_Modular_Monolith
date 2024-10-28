using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Databases.Configurations.DiamondShapeConfig;
using FluentEmail.Core;
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
            string diamondKey = GetPriceKey(entity.ShapeId.Value, true);
            string diamondKeyNatural = GetPriceKey(entity.ShapeId.Value, false);
            _cache.Remove(diamondKey);
            _cache.Remove(diamondKeyNatural);
        }
        public override async Task Update(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Update(entity, token);
            string diamondKey = GetPriceKey(entity.ShapeId.Value, true);
            string diamondKeyNatural = GetPriceKey(entity.ShapeId.Value, false);
            _cache.Remove(diamondKey);
            _cache.Remove(diamondKeyNatural);
        }
        public override async Task Delete(DiamondPrice entity, CancellationToken token = default)
        {
            await base.Delete(entity, token);
            string diamondKey = GetPriceKey(entity.ShapeId.Value, true);
            string diamondKeyNatural = GetPriceKey(entity.ShapeId.Value, false);
            _cache.Remove(diamondKey);
            _cache.Remove(diamondKeyNatural);
        }

        public async Task<List<DiamondPrice>> GetPriceByShapes(DiamondShape shape, bool? isLabDiamond = null, CancellationToken token = default)
        {
            if (isLabDiamond != null)
            {
                string diamondKey = GetPriceKey(shape.Id.Value, isLabDiamond.Value);
                var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
                if (tryGet == null)
                {
                    var get = await _set.Include(p => p.Criteria)
                        .Where(p => p.ShapeId == shape.Id && p.IsLabDiamond == isLabDiamond)
                        .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKey, get);
                    return get;
                }
                return tryGet;
            }
            else
            {
                string diamondKey = GetPriceKey(shape.Id.Value, true);
                string diamondKeyNatural = GetPriceKey(shape.Id.Value, false);
                List<DiamondPrice> results = new();
                var tryGet = _cache.Get<List<DiamondPrice>>(diamondKey);
                var tryGetNatural = _cache.Get<List<DiamondPrice>>(diamondKeyNatural);
                if (tryGet is null || tryGet.Count == 0)
                {
                    var getFromDb = await _set.Include(p => p.Criteria)
                      .Where(p => p.ShapeId == shape.Id && p.IsLabDiamond == true)
                      .OrderBy(s => s.Criteria.CaratTo).ToListAsync();
                    _cache.Set(diamondKey, getFromDb);
                    results.AddRange(getFromDb);
                }
                else
                    results.AddRange(tryGet);
                if (tryGetNatural is null || tryGetNatural.Count == 0)
                {
                    var getFromDb = await _set.Include(p => p.Criteria)
                      .Where(p => p.ShapeId == shape.Id && p.IsLabDiamond == false)
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
                return _set.Where(d => d.CriteriaId == diamondCriteriaId && d.IsLabDiamond == isLabDiamond).ToListAsync();
            
            else
                return _set.Where(d => d.CriteriaId == diamondCriteriaId).ToListAsync();
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
    }
}
