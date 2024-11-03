using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetSelling
{
    public record GetSellingJewelryQuery(int page, string ModelId, string MetalId, string SizeId, string? SideDiamondOptId, decimal? MinPrice = null, decimal? MaxPrice = null) : IRequest<Result<PagingResponseDto<Jewelry>>>;
    internal class GetSellingJewelryQueryHandler : IRequestHandler<GetSellingJewelryQuery, Result<PagingResponseDto<Jewelry>>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryModelService _jewelryModelService;
        private readonly IJewelryService _jewelryService;

        public GetSellingJewelryQueryHandler(IJewelryRepository jewelryRepository, IJewelryService jewelryService, ISizeMetalRepository sizeMetalRepository, IJewelryModelService jewelryModelService, IDiamondRepository diamondRepository)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryService = jewelryService;
            _sizeMetalRepository = sizeMetalRepository;
            _jewelryModelService = jewelryModelService;
            _diamondRepository = diamondRepository;
        }

        public async Task<Result<PagingResponseDto<Jewelry>>> Handle(GetSellingJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out int page, out string modelId, out string metalId, out string sizeId, out string? sideDiamondOptId, out decimal? minPrice, out decimal? maxPrice);
            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);
            sizeMetalQuery = _sizeMetalRepository.QueryFilter(sizeMetalQuery,
                p =>
                p.MetalId == MetalId.Parse(metalId) && p.ModelId == JewelryModelId.Parse(modelId) && p.SizeId == SizeId.Parse(sizeId)
                );
            var sizeMetal = sizeMetalQuery.FirstOrDefault();
            if (sizeMetal == null)
                return Result.Fail("Can't get the requirement of this jewelry list");
            var jewelryQuery = _jewelryService.GetJewelryQueryFromModel(JewelryModelId.Parse(modelId), MetalId.Parse(metalId), SizeId.Parse(sizeId));
            var jewelries = jewelryQuery.ToList();
            var addPriceFlag = _jewelryService.SetupUnmapped(jewelries, sizeMetal);
            if (!addPriceFlag)
                return Result.Fail("Can't get jewelries' price");
            jewelries = jewelries.Where(p =>
            {
                bool flag = true;
                if (minPrice != null)
                    flag = flag && p.TotalPrice >= minPrice;
                if (maxPrice != null)
                    flag = flag && p.TotalPrice <= maxPrice;
                return flag;
            }).Skip(page * JewelryRule.MinimumItemPerPaging).Take(JewelryRule.MinimumItemPerPaging).ToList();
            return new PagingResponseDto<Jewelry>(0, page, jewelries);
        }
    }
}
