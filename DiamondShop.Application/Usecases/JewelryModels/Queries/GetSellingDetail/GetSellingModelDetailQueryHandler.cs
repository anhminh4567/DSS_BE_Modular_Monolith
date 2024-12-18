using DiamondShop.Application.Services.Interfaces.JewelryModels;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;
using FluentValidation.Results;
using MediatR;

namespace DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail
{
    public record GetSellingModelDetailQuery(string modelId) : IRequest<Result<JewelryModelSellingDetail>>;
    internal class GetSellingModelDetailQueryHandler : IRequestHandler<GetSellingModelDetailQuery, Result<JewelryModelSellingDetail>>
    {

        private readonly IJewelryRepository _jewelryRepository;
        private readonly IJewelryModelRepository _modelRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IJewelryModelFileService _jewelryModelFileService;
        private readonly IDiscountRepository _discountRepository;

        public GetSellingModelDetailQueryHandler(IJewelryRepository jewelryRepository, IJewelryModelRepository modelRepository, IDiamondServices diamondServices, IJewelryModelFileService jewelryModelFileService, IDiscountRepository discountRepository)
        {
            _jewelryRepository = jewelryRepository;
            _modelRepository = modelRepository;
            _diamondServices = diamondServices;
            _jewelryModelFileService = jewelryModelFileService;
            _discountRepository = discountRepository;
        }

        public async Task<Result<JewelryModelSellingDetail>> Handle(GetSellingModelDetailQuery request, CancellationToken token)
        {
            var activeDiscount = await _discountRepository.GetActiveDiscount();
            request.Deconstruct(out string modelId);
            var query = _modelRepository.GetSellingModelQuery();
            query = _modelRepository.IncludeMainDiamondQuery(query);
            query = _modelRepository.QueryFilter(query, p => p.Id == JewelryModelId.Parse(modelId));
            var model = query.FirstOrDefault();
            if (model == null)
                return Result.Fail(JewelryModelErrors.JewelryModelNotFoundError);
            var sideDiamonds = model.SideDiamonds;
            //SizeMetal
            var metalGroup = model.SizeMetals
                .GroupBy(p => p.Metal);
            List<Metal> metalList = metalGroup.Select(p => p.Key).ToList();
            List<SellingDetailMetal> metalGroups = new();
            var medias = await _jewelryModelFileService.GetFolders(model, token);
            var gallery = _jewelryModelFileService.MapPathsToCorrectGallery(model, medias, token);
            foreach (var metals in metalGroup)
            {
                if (sideDiamonds != null && sideDiamonds.Count > 0)
                {
                    foreach (var side in sideDiamonds)
                    {
                        var key = $"Categories/{metals.Key.Id.Value}/{side.Id.Value}";
                        gallery.Gallery.TryGetValue(key, out List<Media>? sideDiamondImages);
                        var sizesInStock = _jewelryRepository.GetSizesInStock(model.Id, metals.Key.Id, side);
                        await _diamondServices.GetSideDiamondPrice(side);
                        metalGroups.Add(
                            SellingDetailMetal.CreateWithSide(
                                model.Name, metals.Key, side.TotalPrice > 0, side, sideDiamondImages,
                                metals.Select(k =>
                                SellingDetailSize.Create(k.Size.Value, k.Size.Unit, side.TotalPrice != null ? side.TotalPrice + model.CraftmanFee + k.Price : 0, sizesInStock.Contains(k.SizeId))).ToList()
                            ));
                    };
                }
                else
                {
                    var images = gallery.BaseMetals.Where(p => p.MediaPath.Contains($"Metals/{metals.Key.Id.Value}")).ToList();
                    var sizesInStock = _jewelryRepository.GetSizesInStock(model.Id, metals.Key.Id);
                    metalGroups.Add(
                        SellingDetailMetal.CreateNoSide(
                            model.Name, metals.Key, images,
                            metals.Select(p => SellingDetailSize.Create(p.Size.Value, p.Size.Unit, p.Price + model.CraftmanFee, sizesInStock.Contains(p.SizeId))).ToList()
                        ));
                }
            }
            var result = JewelryModelSellingDetail.Create(model, metalGroups, sideDiamonds, metalList, null);
            // assign discount
            result.AssignDiscount(activeDiscount);
            return result;
        }
    }
}