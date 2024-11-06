using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetJewelryDiamond
{
    public record GetJewelryDiamondQuery(int CurrentPage, int PageSize, string ModelId, string MetalId, string SizeId, string? SideDiamondOptId, decimal? MinPrice = null, decimal? MaxPrice = null) : IRequest<Result<PagingResponseDto<Jewelry>>>;
    internal class GetJewelryDiamondQueryHandler : IRequestHandler<GetJewelryDiamondQuery, Result<PagingResponseDto<Jewelry>>>
    {
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly ISideDiamondRepository _sideDiamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryModelService _jewelryModelService;
        private readonly IJewelryService _jewelryService;

        public GetJewelryDiamondQueryHandler(IJewelryService jewelryService, ISizeMetalRepository sizeMetalRepository, IJewelryModelService jewelryModelService, IDiamondRepository diamondRepository, ISideDiamondRepository sideDiamondRepository, IJewelryModelRepository jewelryModelRepository)
        {
            _jewelryService = jewelryService;
            _sizeMetalRepository = sizeMetalRepository;
            _jewelryModelService = jewelryModelService;
            _diamondRepository = diamondRepository;
            _sideDiamondRepository = sideDiamondRepository;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result<PagingResponseDto<Jewelry>>> Handle(GetJewelryDiamondQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int currentPage, out int pageSize, out string modelId, out string metalId, out string sizeId, out string? sideDiamondOptId, out decimal? minPrice, out decimal? maxPrice);
            pageSize = pageSize == 0 ? JewelryRule.MinimumItemPerPaging : 1;
            var model = await _jewelryModelRepository.GetSellingModelDetail(JewelryModelId.Parse(modelId), MetalId.Parse(metalId), SizeId.Parse(sizeId));
            if (model == null)
                return Result.Fail("Can't get the requested model");

            var sizeMetal = model.SizeMetals.FirstOrDefault(p => p.MetalId == MetalId.Parse(metalId) && p.SizeId == SizeId.Parse(sizeId));
            if (sizeMetal == null)
                return Result.Fail("The size metal option for this model doesn't exist");

            SideDiamondOpt? sideDiamond = null;
            if (model.SideDiamonds.Count > 0)
            {
                if (sideDiamondOptId == null)
                    return Result.Fail("This model requires side diamond selected");
                sideDiamond = model.SideDiamonds.FirstOrDefault(p => p.Id == SideDiamondOptId.Parse(sideDiamondOptId));
                if (sideDiamond == null)
                    return Result.Fail("Incorrect side diamond option for this model");
            }

            var jewelryQuery = _jewelryService.GetJewelryQueryFromModel(model.Id, sizeMetal.MetalId, sizeMetal.SizeId, sideDiamond);

            int maxPage = (int)Math.Ceiling((decimal)jewelryQuery.Count() / pageSize);
            var jewelries = jewelryQuery.Skip(pageSize * currentPage).Take(pageSize).ToList();
            var addPriceFlag = _jewelryService.SetupUnmapped(jewelries, sizeMetal);
            if (!addPriceFlag)
                return Result.Fail("Can't get jewelries' price");
            jewelries = jewelries.Where(p =>
            {
                bool flag = true;
                if (minPrice != null)
                    flag = flag && p.D_Price >= minPrice;
                if (maxPrice != null)
                    flag = flag && p.D_Price <= maxPrice;
                return flag;
            }).Skip(currentPage * pageSize).Take(pageSize).ToList();
            return new PagingResponseDto<Jewelry>(maxPage, currentPage, jewelries);
        }
    }
}