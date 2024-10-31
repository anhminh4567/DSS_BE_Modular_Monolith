using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.Jewelries.Queries.GetSelling
{
    public record GetSellingJewelryQuery(string ModelId, string MetalId, string SizeId, string? SideDiamondOptId) : IRequest<Result<List<Jewelry>>>;
    internal class GetSellingJewelryQueryHandler : IRequestHandler<GetSellingJewelryQuery, Result<List<Jewelry>>>
    {
        private readonly IJewelryRepository _jewelryRepository;
        private readonly ISizeMetalRepository _sizeMetalRepository;
        private readonly IJewelryModelService _jewelryModelService;
        private readonly IJewelryService _jewelryService;

        public GetSellingJewelryQueryHandler(IJewelryRepository jewelryRepository, IJewelryService jewelryService, ISizeMetalRepository sizeMetalRepository, IJewelryModelService jewelryModelService)
        {
            _jewelryRepository = jewelryRepository;
            _jewelryService = jewelryService;
            _sizeMetalRepository = sizeMetalRepository;
            _jewelryModelService = jewelryModelService;
        }

        public async Task<Result<List<Jewelry>>> Handle(GetSellingJewelryQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string modelId, out string metalId, out string sizeId, out string? sideDiamondOptId);
            var sizeMetalQuery = _sizeMetalRepository.GetQuery();
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery, p => p.Metal);
            sizeMetalQuery = _sizeMetalRepository.QueryInclude(sizeMetalQuery,
                p =>
                p.MetalId == MetalId.Parse(metalId) && p.ModelId == JewelryModelId.Parse(modelId) && p.SizeId == SizeId.Parse(sizeId)
                );
            var sizeMetal = sizeMetalQuery.FirstOrDefault();
            if (sizeMetal == null)
                return Result.Fail("Can't get the requirement of this jewelry list");

            var jewelryQuery = _jewelryService.GetJewelryQueryFromModel(JewelryModelId.Parse(modelId), MetalId.Parse(metalId), SizeId.Parse(sizeId), _jewelryRepository);
            var jewelries = jewelryQuery.Take(JewelryRule.MinimumItemPerPaging).ToList();
            _jewelryService.SetupUnmapped(jewelries, sizeMetal);
            return jewelries;
        }
    }
}
