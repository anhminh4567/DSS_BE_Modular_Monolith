﻿using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling
{
    public record GetSellingModelQuery(int page = 0, string? Category = null, string? MetalId = null, decimal? MinPrice = null, decimal? MaxPrice = null, bool? IsRhodiumFinished = null, bool? IsEngravable = null) : IRequest<Result<PagingResponseDto<JewelryModelSellingDto>>>;
    internal class GetSellingModelQueryHandler : IRequestHandler<GetSellingModelQuery, Result<PagingResponseDto<JewelryModelSellingDto>>>
    {
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IMainDiamondRepository _mainDiamondRepository;
        private readonly IJewelryModelService _jewelryModelService;

        public GetSellingModelQueryHandler(
            ISizeMetalRepository sizeMetalRepository,
            IJewelryModelRepository modelRepository,
            IMainDiamondRepository mainDiamondRepository, IJewelryModelCategoryRepository categoryRepository, IJewelryModelService jewelryModelService)
        {
            _sizeMetalRepository = sizeMetalRepository;
            _modelRepository = modelRepository;
            _mainDiamondRepository = mainDiamondRepository;
            _categoryRepository = categoryRepository;
            _jewelryModelService = jewelryModelService;
        }

        public async Task<Result<PagingResponseDto<JewelryModelSellingDto>>> Handle(GetSellingModelQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int page, out string? Category, out string? metalId, out decimal? minPrice, out decimal? maxPrice, out bool? isRhodiumFinished, out bool? isEngravable);
            var query = _modelRepository.GetSellingModel();
            if (!string.IsNullOrEmpty(Category))
            {
                var category = await _categoryRepository.ContainsName(Category);
                if (category == null)
                {
                    return BlankPaging();
                }
                query = _modelRepository.QueryFilter(query, p => p.CategoryId == category.Id);
            }
            if (isRhodiumFinished != null)
            {
                query = _modelRepository.QueryFilter(query, p => p.IsRhodiumFinish == isRhodiumFinished);
            }
            if (isEngravable != null)
            {
                query = _modelRepository.QueryFilter(query, p => p.IsEngravable == isEngravable);
            }
            List<JewelryModelSellingDto> sellingModels = new();
            var pageIndex = GetData(sellingModels, query, page, metalId, minPrice, maxPrice);
            return new PagingResponseDto<JewelryModelSellingDto>(0, pageIndex, sellingModels);
        }
        private PagingResponseDto<JewelryModelSellingDto> BlankPaging() => new PagingResponseDto<JewelryModelSellingDto>(0, 0, []);
        private int GetData(List<JewelryModelSellingDto> sellingModels, IQueryable<JewelryModel> query, int page, string? metalId, decimal? minPrice, decimal? maxPrice)
        {
            //TODO: REplace diamond price with real price getter
            const decimal DiamondPrices = 10m;
            var models = query.Skip(JewelryModelRule.ModelPerQuery * page).Take(JewelryModelRule.ModelPerQuery).ToList();
            if (models.Count == 0)
                return page == 0 ? 0 : page - 1;
            foreach (var model in models)
            {
                var sideDiamonds = model.SideDiamonds;
                sideDiamonds.ForEach(d => d.Price = DiamondPrices);
                var sizeMetals = model.SizeMetals
                    .Where(p =>
                    {
                        if (metalId != null)
                            return p.MetalId.Value == metalId;
                        else return true;
                    })
                    .GroupBy(p => p.Metal)
                    .Select(p =>
                    {
                        var min = p.MinBy(k => k.Weight);
                        var max = p.MaxBy(k => k.Weight);
                        if (min != null || max != null)
                        {
                            _jewelryModelService.GetSizeMetalPrice(min);
                            _jewelryModelService.GetSizeMetalPrice(max);
                        }
                        return new
                        {
                            GroupName = p.Key.Name,
                            Min = min,
                            Max = max,
                        };
                    });
                foreach (var sizeMetal in sizeMetals)
                {
                    if (sideDiamonds != null && sideDiamonds.Count > 0)
                    {
                        var created_side = sideDiamonds.Select(p => new JewelryModelSellingDto()
                        {
                            Name = $"{model.Name} in {sizeMetal.GroupName} ({p.CaratWeight} Tw)",
                            ThumbnailPath = "",
                            StarRating = 0,
                            ReviewCount = 0,
                            MinPrice = p.Price + model.CraftmanFee + sizeMetal.Min.Price,
                            MaxPrice = p.Price + model.CraftmanFee + sizeMetal.Max.Price,
                        })
                        .Where(p =>
                        {
                            bool flag = true;
                            if (maxPrice != null)
                                flag = flag && (p.MinPrice <= maxPrice);
                            if (minPrice != null)
                                flag = flag && (p.MaxPrice >= minPrice);
                            return flag;
                        });
                        sellingModels.AddRange(created_side);
                    }
                    else
                    {
                        var created_noside = new JewelryModelSellingDto()
                        {
                            Name = $"{model.Name} in {sizeMetal.GroupName}",
                            ThumbnailPath = "",
                            StarRating = 0,
                            ReviewCount = 0,
                            MinPrice = model.CraftmanFee + sizeMetal.Min.Price,
                            MaxPrice = model.CraftmanFee + sizeMetal.Max.Price,
                        };
                        if (maxPrice != null)
                            if (created_noside.MinPrice > maxPrice) continue;
                        if (minPrice != null)
                            if (created_noside.MaxPrice < minPrice) continue;
                        sellingModels.Add(created_noside);
                    }
                }
            }
            if (sellingModels.Count < JewelryModelRule.MinimumModelPerPaging)
                return GetData(sellingModels, query, page + 1, metalId, minPrice, maxPrice);
            else
                return page;
        }
    }
}
