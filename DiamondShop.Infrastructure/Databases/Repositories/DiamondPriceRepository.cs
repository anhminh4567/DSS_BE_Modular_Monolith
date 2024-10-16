using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
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
            DiamondShapeId shapeId = (DiamondShapeId) ids[0];
            DiamondCriteriaId criteriaId  = (DiamondCriteriaId)ids[1];
            return _set.FirstOrDefaultAsync(d => d.CriteriaId == criteriaId && d.ShapeId == shapeId);
        }
        public Task<DiamondPrice?> GetById(DiamondShapeId shapeId, DiamondCriteriaId criteriaId, CancellationToken cancellationToken = default)
        {
            return _set.Include(p => p.Criteria).Include(p => p.Shape).FirstOrDefaultAsync(d => d.CriteriaId == criteriaId && d.ShapeId == shapeId,cancellationToken);
        }
        public override async Task<List<DiamondPrice>> GetAll(CancellationToken token = default)
        {
            var getFromDb = await _dbContext.DiamondPrices.Include(p => p.Criteria).ToListAsync() ;
            return getFromDb;
        }
        public override Task Create(DiamondPrice entity, CancellationToken token = default)
        {
            return base.Create(entity, token);
        }
        public override Task Update(DiamondPrice entity, CancellationToken token = default)
        {
            return base.Update(entity, token);
        }
        public override Task Delete(DiamondPrice entity, CancellationToken token = default)
        {
            return base.Delete(entity, token);
        }

        public async Task<List<DiamondPrice>> GetPriceByShapes(DiamondShape shape, CancellationToken token = default)
        {
            string diamondKey = $"DP_{shape.Shape}";
            var tryGet =  _cache.Get<List<DiamondPrice>>(diamondKey);
            if(tryGet == null) 
            {
                var get = await _set.Include(p => p.Criteria).Where(p => p.ShapeId == shape.Id).ToListAsync();
                _cache.Set(diamondKey, get);
                return get;
            }
            return tryGet;
        }

        public Task CreateMany(List<DiamondPrice> prices)
        {
            return _set.AddRangeAsync(prices);
        }

        public Task<List<DiamondPrice>> GetPriceByCriteria(DiamondCriteriaId diamondCriteriaId, CancellationToken token = default)
        {
            return _set.Where(d => d.CriteriaId == diamondCriteriaId).ToListAsync();
        }
    }
}
