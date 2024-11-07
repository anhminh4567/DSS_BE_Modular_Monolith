using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail
{
    public record GetSellingModelDetailQuery(string modelId) : IRequest<Result<JewelryModelSellingDetail>>;
    internal class GetSellingModelDetailQueryHandler : IRequestHandler<GetSellingModelDetailQuery, Result<JewelryModelSellingDetail>>
    {

        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly IJewelryModelService _modelService;
        private readonly IDiamondServices _diamondServices;
        public GetSellingModelDetailQueryHandler(IJewelryModelRepository modelRepository, IJewelryModelService modelService, IJewelryRepository jewelryRepository, IDiamondServices diamondServices)
        {
            _modelRepository = modelRepository;
            _modelService = modelService;
            _jewelryRepository = jewelryRepository;
            _diamondServices = diamondServices;
        }

        public async Task<Result<JewelryModelSellingDetail>> Handle(GetSellingModelDetailQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string modelId);
            var query = _modelRepository.GetSellingModelQuery();
            query = _modelRepository.IncludeMainDiamondQuery(query);
            query = _modelRepository.QueryFilter(query, p => p.Id == JewelryModelId.Parse(modelId));
            var model = query.FirstOrDefault();
            if (model == null)
                return Result.Fail("This model doesn't exist");

            //TODO: REplace diamond price with real price getter
            const decimal DiamondPrices = 10m;
            var sideDiamonds = model.SideDiamonds;
            //sideDiamonds.ForEach(d => d.TotalPrice = DiamondPrices);

            //SizeMetal
            var metalGroup = model.SizeMetals
                .GroupBy(p => p.Metal);
            List<Metal> metalList = metalGroup.Select(p => p.Key).ToList();
            List<SellingDetailMetal> metalGroups = new();
            foreach (var metals in metalGroup)
            {
                if (sideDiamonds != null && sideDiamonds.Count > 0)
                {
                    foreach(var side in sideDiamonds)
                    {
                        //side.TotalPrice = DiamondPrices;
                        var sizesInStock = _jewelryRepository.GetSizesInStock(model.Id, metals.Key.Id, side);
                        await _diamondServices.GetSideDiamondPrice(side);
                        metalGroups.Add(
                            SellingDetailMetal.CreateWithSide(
                                model.Name, metals.Key, side.TotalPrice > 0, side, null,
                                metals.Select(k =>
                                SellingDetailSize.Create(k.Size.Value,side.TotalPrice != null ? side.TotalPrice + model.CraftmanFee + k.Price : 0, sizesInStock.Contains(k.SizeId))).ToList()
                            ));
                    };
                }
                else
                {
                    var sizesInStock = _jewelryRepository.GetSizesInStock(model.Id, metals.Key.Id);
                    metalGroups.Add(
                        SellingDetailMetal.CreateNoSide(
                            model.Name, metals.Key, null,
                            metals.Select(p => SellingDetailSize.Create(p.Size.Value, p.Price + model.CraftmanFee, sizesInStock.Contains(p.SizeId))).ToList()
                        ));
                }
            }
            return JewelryModelSellingDetail.Create(model, metalGroups, sideDiamonds, metalList, null);
        }
    }
}