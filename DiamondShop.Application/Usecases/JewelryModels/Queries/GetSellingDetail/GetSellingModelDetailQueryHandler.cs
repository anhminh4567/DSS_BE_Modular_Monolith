using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail
{
    public record GetSellingModelDetailQuery(string modelId) : IRequest<Result<JewelryModelSellingDetail>>;
    internal class GetSellingModelDetailQueryHandler : IRequestHandler<GetSellingModelDetailQuery, Result<JewelryModelSellingDetail>>
    {

        private readonly IJewelryModelRepository _modelRepository;
        private readonly IJewelryModelService _modelService;

        public GetSellingModelDetailQueryHandler(IJewelryModelRepository modelRepository, IJewelryModelService modelService)
        {
            _modelRepository = modelRepository;
            _modelService = modelService;
        }

        public async Task<Result<JewelryModelSellingDetail>> Handle(GetSellingModelDetailQuery request, CancellationToken cancellationToken)
        {
            request.Deconstruct(out string modelId);
            var query = _modelRepository.GetSellingModelQuery();
            query = _modelRepository.QueryFilter(query, p => p.Id == JewelryModelId.Parse(modelId));
            var model = query.FirstOrDefault();
            if (model == null)
                return Result.Fail("This model doesn't exist");

            //TODO: REplace diamond price with real price getter
            const decimal DiamondPrices = 10m;
            var sideDiamonds = model.SideDiamonds;
            sideDiamonds.ForEach(d => d.Price = DiamondPrices);

            //SizeMetal
            _modelService.GetSizeMetalPrice(model.SizeMetals);
            var metalGroup = model.SizeMetals
                .GroupBy(p => p.Metal);

            List<SellingDetailMetal> metalGroups = new();
            foreach (var metals in metalGroup)
            {
                if (sideDiamonds != null && sideDiamonds.Count > 0)
                {
                    sideDiamonds.ForEach(p =>
                    {
                        p.Price = DiamondPrices;
                        metalGroups.Add(
                            SellingDetailMetal.CreateWithSide(
                                model.Name, metals.Key, p, null,
                                metals.Select(k => SellingDetailSize.Create(k.Size.Value, p.Price + model.CraftmanFee + k.Price)).ToList()
                            ));
                    });
                }
                else
                {
                    metalGroups.Add(
                        SellingDetailMetal.CreateNoSide(
                            model.Name, metals.Key, null,
                            metals.Select(p => SellingDetailSize.Create(p.Size.Value, p.Price + model.CraftmanFee)).ToList()
                        ));
                }
            }
            return JewelryModelSellingDetail.Create(model, metalGroups, sideDiamonds, null);
        }
    }
}