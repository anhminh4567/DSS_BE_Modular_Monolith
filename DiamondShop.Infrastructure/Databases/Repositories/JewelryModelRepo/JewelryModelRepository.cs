﻿using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryModelRepo
{
    internal class JewelryModelRepository : BaseRepository<JewelryModel>, IJewelryModelRepository
    {
        private readonly IMemoryCache _cache;
        public JewelryModelRepository(DiamondShopDbContext dbContext, IMemoryCache cache) : base(dbContext) 
        { 
            _cache = cache;
        }

        public override async Task<JewelryModel?> GetById(params object[] ids)
        {
            JewelryModelId id = (JewelryModelId)ids[0];
            return await _set.Include(p => p.Category).Include(p => p.MainDiamonds).Include(p => p.SideDiamonds).Include(p => p.SizeMetals)
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public override async Task<List<JewelryModel>> GetAll(CancellationToken token = default)
        {
            var getFromDb = await _dbContext.JewelryModels.Include(p => p.Category).Include(p => p.MainDiamonds).Include(p => p.SideDiamonds).ToListAsync();
            return getFromDb;
        }

        public async Task<JewelryModel?> GetByIdMinimal(JewelryModelId id)
        {
            return await _set.FirstOrDefaultAsync(s => s.Id == id);
        }

        public IQueryable<JewelryModel> GetSellingModelQuery()
        {
            var query = _set.AsQueryable();
            query = query.Include(p => p.Category);
            query = query.Include(p => p.SizeMetals).ThenInclude(p => p.Metal);
            query = query.Include(p => p.SizeMetals).ThenInclude(p => p.Size);
            query = query.Include(p => p.SideDiamonds);
            return query.AsSplitQuery();
        }

    }
}
