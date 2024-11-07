﻿using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace DiamondShop.Domain.Repositories
{
    public record DeleteManyParameter(DiamondShapeId DiamondShapeId, DiamondCriteriaId CriteriaId);
    public interface IDiamondPriceRepository : IBaseRepository<DiamondPrice>
    {
        Task<List<DiamondPrice>> GetPriceByShapes(DiamondShape shape, bool? isLabDiamond = null, CancellationToken token = default);
        Task<List<DiamondPrice>> GetPriceByCriteria(DiamondCriteriaId diamondCriteriaId, bool? isLabDiamond = null, CancellationToken token = default);
        Task<List<DiamondPrice>> GetPrice(bool isFancyShape, bool? isLabDiamond = null, CancellationToken token = default);
        Task<List<DiamondPrice>> GetPriceIgnoreCache(bool isFancyShape, bool? isLabDiamond = null, CancellationToken token = default);

        Task<DiamondPrice?> GetById(DiamondShapeId shapeId, DiamondCriteriaId criteriaId, CancellationToken cancellationToken =default);
        Task CreateMany(List<DiamondPrice> prices);
        //Task<List<DiamondPrice>> GetSideDiamondPriceByShape(DiamondShape shape, bool? islabDiamond = null, CancellationToken cancellationToken = default);
        Task<List<DiamondPrice>> GetSideDiamondPrice(bool isFancyShape,bool isLabDiamond,CancellationToken token = default);
        Task<List<DiamondPrice>> GetSideDiamondPriceByAverageCarat(bool isFancyShape, bool isLabDiamond, float avgCarat, CancellationToken token = default);
        Task<Result> DeleteMany(List<DeleteManyParameter> parameters , bool Islab, bool IsSide, CancellationToken cancellationToken = default);
        void RemoveAllCache();

    }
}
