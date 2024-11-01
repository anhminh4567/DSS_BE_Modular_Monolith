﻿using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo
{
    internal class JewelryRepository : BaseRepository<Jewelry>, IJewelryRepository
    {
        private readonly IMemoryCache _cache;
        public JewelryRepository(DiamondShopDbContext dbContext, IMemoryCache cache) : base(dbContext)
        {
            _cache = cache;
        }
        public override async Task<Jewelry?> GetById(params object[] ids)
        {
            JewelryId id = (JewelryId)ids[0];
            return await _set.Include(d => d.SideDiamond)
                .Include(d => d.Diamonds)
                .ThenInclude(d => d.DiamondShape).FirstOrDefaultAsync(d => d.Id == id);
        }
        public void UpdateRange(List<Jewelry> jewelries)
        {
            _set.UpdateRange(jewelries);
        }

        public async Task<bool> CheckDuplicatedSerial(string serialNumber)
        {
            return await _set.AnyAsync(p => p.SerialCode == serialNumber);
        }

        public Task<bool> IsHavingDiamond(Jewelry jewelry, CancellationToken cancellationToken = default)
        {
            return _dbContext.Diamonds.AnyAsync(d => d.JewelryId == jewelry.Id, cancellationToken);
        }

        public IQueryable<SizeId> GetSizesInStock(JewelryModelId modelId, MetalId metalId,
            SideDiamondOpt sideDiamondOpt)
        {
            return _set.Where(p =>
            p.Status == ProductStatus.Active && p.ModelId == modelId && p.MetalId == metalId &&
            p.SideDiamond.ColorMin == sideDiamondOpt.ColorMin && p.SideDiamond.ColorMax == sideDiamondOpt.ColorMax &&
            p.SideDiamond.ClarityMin == sideDiamondOpt.ClarityMin && p.SideDiamond.ClarityMax == sideDiamondOpt.ClarityMax &&
            p.SideDiamond.SettingType == sideDiamondOpt.SettingType && p.SideDiamond.Carat == sideDiamondOpt.CaratWeight && p.SideDiamond.Quantity == sideDiamondOpt.Quantity && p.SideDiamond.DiamondShapeId == sideDiamondOpt.ShapeId
            ).Select(p => p.SizeId).Distinct();
        }
        public IQueryable<SizeId> GetSizesInStock(JewelryModelId modelId, MetalId metalId)
        {
            return _set.Where(p =>
                p.Status == ProductStatus.Active && p.ModelId == modelId && p.MetalId == metalId
            ).Select(p => p.SizeId).Distinct();
        }
        public async Task<bool> Existing(JewelryModelId modelId, MetalId metalId, SizeId sizeId)
        {
            return await _set.Where(p => p.Status == ProductStatus.Active && p.ModelId == modelId && p.MetalId == metalId && p.SizeId == sizeId).AnyAsync();
        }
        public async Task<bool> Existing(JewelryModelId modelId, SideDiamondOpt sideDiamondOpt)
        {
            return await _set.Where(p => p.Status == ProductStatus.Active && p.ModelId == modelId &&
            p.SideDiamond.Carat == sideDiamondOpt.CaratWeight && p.SideDiamond.SettingType == sideDiamondOpt.SettingType &&
            p.SideDiamond.Quantity == sideDiamondOpt.Quantity && p.SideDiamond.DiamondShapeId == sideDiamondOpt.ShapeId &&
            p.SideDiamond.ColorMin == sideDiamondOpt.ColorMin && p.SideDiamond.ColorMax == sideDiamondOpt.ColorMax &&
            p.SideDiamond.ClarityMax == sideDiamondOpt.ClarityMax && p.SideDiamond.ClarityMax == sideDiamondOpt.ClarityMax
            ).AnyAsync();
        }
    }
}
