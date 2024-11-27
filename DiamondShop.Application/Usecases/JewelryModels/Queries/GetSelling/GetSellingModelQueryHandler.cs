﻿using DiamondShop.Application.Commons.Models;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling
{
    public record GetSellingModelQuery(int page = 0, string? Category = null, string? MetalId = null, decimal? MinPrice = null, decimal? MaxPrice = null, bool? IsRhodiumFinished = null, bool? IsEngravable = null) : IRequest<Result<PagingResponseDto<JewelryModelSelling>>>;
    internal class GetSellingModelQueryHandler : IRequestHandler<GetSellingModelQuery, Result<PagingResponseDto<JewelryModelSelling>>>
    {
        private readonly Dictionary<string, JewelryModelGalleryTemplate?> cachedGallery = new();
        private readonly IJewelryModelCategoryRepository _categoryRepository;
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IDiamondServices _diamondServices;

        public GetSellingModelQueryHandler(IJewelryModelCategoryRepository categoryRepository, IJewelryRepository jewelryRepository, IJewelryModelRepository jewelryModelRepository, IDiamondServices diamondServices, IJewelryModelFileService jewelryModelFileService)
        {
            _categoryRepository = categoryRepository;
            _jewelryRepository = jewelryRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _diamondServices = diamondServices;
            _jewelryModelFileService = jewelryModelFileService;
        }

        public async Task<Result<PagingResponseDto<JewelryModelSelling>>> Handle(GetSellingModelQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int page, out string? Category, out string? metalId, out decimal? minPrice, out decimal? maxPrice, out bool? isRhodiumFinished, out bool? isEngravable);
            var query = _jewelryModelRepository.GetSellingModelQuery();
            if (!string.IsNullOrEmpty(Category))
            {
                var category = await _categoryRepository.ContainsName(Category);
                if (category == null)
                {
                    return BlankPaging();
                }
                query = _jewelryModelRepository.QueryFilter(query, p => p.CategoryId == category.Id);
            }
            if (isRhodiumFinished != null)
            {
                query = _jewelryModelRepository.QueryFilter(query, p => p.IsRhodiumFinish == isRhodiumFinished);
            }
            if (isEngravable != null)
            {
                query = _jewelryModelRepository.QueryFilter(query, p => p.IsEngravable == isEngravable);
            }
            List<JewelryModelSelling> sellingModels = new();
            var pageIndex = await GetData(sellingModels, query, page, metalId, minPrice, maxPrice);
            return new PagingResponseDto<JewelryModelSelling>(0, pageIndex, sellingModels);
        }
        private PagingResponseDto<JewelryModelSelling> BlankPaging() => new PagingResponseDto<JewelryModelSelling>(0, 0, []);
        private async Task<int> GetData(List<JewelryModelSelling> sellingModels, IQueryable<JewelryModel> query, int page, string? metalId, decimal? minPrice, decimal? maxPrice)
        {
            var models = query.Skip(JewelryModelRule.ModelPerQuery * page).Take(JewelryModelRule.ModelPerQuery).ToList();
            if (models.Count == 0)
                return page == 0 ? 0 : page - 1;
            foreach (var model in models)
            {
                var sideDiamonds = model.SideDiamonds;
                foreach (var side in sideDiamonds)
                {
                    await _diamondServices.GetSideDiamondPrice(side);
                }
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
                        return new
                        {
                            Metal = p.Key,
                            Min = min,
                            Max = max,
                        };
                    });
                var gallery = await GetModelGallery(model);
                foreach (var sizeMetal in sizeMetals)
                {
                    //check if model has product
                    if (_jewelryRepository.GetSizesInStock(model.Id, sizeMetal.Metal.Id).Count() == 0)
                        continue;
                    if (sideDiamonds != null && sideDiamonds.Count > 0)
                    {
                        var created_side = sideDiamonds
                            .Select(p =>
                            {
                                var key = $"Categories/{sizeMetal.Metal.Id.Value}/{p.Id.Value}";
                                gallery.Gallery.TryGetValue(key, out List<Media>? sideDiamondImages);
                                var thumbnail = sideDiamondImages?.FirstOrDefault();
                                if (sideDiamondImages != null && sideDiamondImages.Count >= 3)
                                    thumbnail = sideDiamondImages[3];
                                return JewelryModelSelling.CreateWithSide(
                            thumbnail, model.Name, sizeMetal.Metal.Name, 0, 0,
                            model.CraftmanFee, sizeMetal.Min.Price, sizeMetal.Max.Price,
                            model.Id, sizeMetal.Metal.Id, p);
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
                        var images = gallery.BaseMetals.Where(p => p.MediaPath.Contains($"Metals/{sizeMetal.Metal.Id.Value}")).ToList();
                        var thumbnail = images.FirstOrDefault();
                        if (images != null && images.Count >= 3)
                            thumbnail = images[3];
                        var created_noside = JewelryModelSelling.CreateNoSide(
                            thumbnail, model.Name, sizeMetal.Metal.Name, 0, 0,
                            model.CraftmanFee, sizeMetal.Min.Price, sizeMetal.Max.Price, model.Id, sizeMetal.Metal.Id);
                        if (maxPrice != null)
                            if (created_noside.MinPrice > maxPrice) continue;
                        if (minPrice != null)
                            if (created_noside.MaxPrice < minPrice) continue;
                        sellingModels.Add(created_noside);
                    }
                }
            }
            if (sellingModels.Count < JewelryModelRule.MinimumItemPerPaging)
                return await GetData(sellingModels, query, page + 1, metalId, minPrice, maxPrice);
            else
                return page;
        }

        private async Task<JewelryModelGalleryTemplate> GetModelGallery(JewelryModel model)
        {
            cachedGallery.TryGetValue(model.Id.Value, out JewelryModelGalleryTemplate? gallery);
            if (gallery is null)
            {
                var medias = await _jewelryModelFileService.GetFolders(model);
                gallery = _jewelryModelFileService.MapPathsToCorrectGallery(model, medias);
                cachedGallery.Add(model.Id.Value, gallery);
                model.Gallery = medias;
            }
            return gallery;
        }
    }
}
