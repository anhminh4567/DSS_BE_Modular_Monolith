using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;

namespace DiamondShop.Domain.Services.Implementations
{
    public class JewelryService : IJewelryService
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IJewelryRepository _jewelryRepository;

        public JewelryService(IDiamondRepository diamondRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository, IDiscountRepository discountRepository, IJewelryRepository jewelryRepository)
        {
            _diamondRepository = diamondRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
            _discountRepository = discountRepository;
            _jewelryRepository = jewelryRepository;
        }
        public string GetSerialCode(JewelryModel model, Metal metal, Size size)
        {
            if (model == null || metal == null || size == null)
                throw new Exception("model or metal or size is null");
            int count = _jewelryRepository.GetSameModelCount(model.Id,metal.Id,size.Id) +1;
            return $"J_{model.ModelCode}-{metal.CodeName}{size.Value}_{count}";
        }

        public bool SetupUnmapped(List<Jewelry> jewelries, List<SizeMetal> sizeMetals)
        {
            foreach (var jewelry in jewelries)
            {
                if (jewelry.Metal == null) return false;
                var sizeMetal = sizeMetals.FirstOrDefault(k => jewelry.ModelId == k.ModelId && jewelry.SizeId == k.SizeId && jewelry.MetalId == k.MetalId);
                if (sizeMetal?.Metal == null)
                    return false;
                jewelry.ND_Price = sizeMetal.Price;
                jewelry.D_Price = GetJewelryDiamondPrice(jewelry).Result;
            }
            return true;
        }
        public bool SetupUnmapped(List<Jewelry> jewelries, SizeMetal sizeMetal)
        {
            foreach (var jewelry in jewelries)
            {
                if (sizeMetal?.Metal == null)
                    return false;
                jewelry.ND_Price = sizeMetal.Price;
                jewelry.D_Price = GetJewelryDiamondPrice(jewelry).Result;
            }
            return true;
        }
        public Jewelry AddPrice(Jewelry jewelry, ISizeMetalRepository sizeMetalRepository)
        {
            var query = sizeMetalRepository.GetQuery();
            if(jewelry.Model == null)
            query = sizeMetalRepository.QueryInclude(query, p => p.Model);
            query = sizeMetalRepository.QueryInclude(query, p => p.Metal);
            query = sizeMetalRepository.QueryFilter(query, p => p.ModelId == jewelry.ModelId && p.SizeId == jewelry.SizeId && p.MetalId == jewelry.MetalId);
            var sizeMetal = query.FirstOrDefault();
            if (sizeMetal != null)
            {   
                jewelry.ND_Price = sizeMetal.Price;
                if (jewelry.Model != null) jewelry.ND_Price += jewelry.Model.CraftmanFee;
                else jewelry.ND_Price += sizeMetal.Model.CraftmanFee;
            }
            jewelry.D_Price = GetJewelryDiamondPrice(jewelry).Result;
            if(jewelry.SideDiamond != null)
                jewelry.SD_Price = GetJewelrySideDiamondPrice(jewelry).Result;
            return jewelry;
        }

        public Jewelry AddPrice(Jewelry jewelry, SizeMetal sizeMetal)
        {
            if (sizeMetal.Metal == null) return null;
            jewelry.ND_Price = sizeMetal.Price;
            jewelry.D_Price = GetJewelryDiamondPrice(jewelry).Result;
            if (jewelry.SideDiamond != null)
                jewelry.SD_Price = GetJewelrySideDiamondPrice(jewelry).Result;
            return jewelry;
        }
        private async Task<decimal> GetJewelryDiamondPrice(Jewelry jewelry)
        {
            var getJewelryDiamonds = await _diamondRepository.GetDiamondsJewelry(jewelry.Id);
            jewelry.Diamonds = getJewelryDiamonds;
            decimal D_price = 0;
            foreach (var diamond in getJewelryDiamonds)
            {
                var prices = await _diamondPriceRepository.GetPrice(diamond.Cut.Value,diamond.DiamondShape, diamond.IsLabDiamond);
                var diamondPrice = _diamondServices.GetDiamondPrice(diamond, prices).Result;
                diamond.DiamondPrice = diamondPrice;
            }
            D_price = jewelry.Diamonds.Sum(d => d.TruePrice);
            jewelry.D_Price = D_price;
            return D_price;
        }
        private async Task<decimal> GetJewelrySideDiamondPrice(Jewelry jewelry)
        {
            var sideDiamond = jewelry.SideDiamond;
            decimal totalDiamondPrice = 0;
            var thisSidePrice = await _diamondPriceRepository.GetSideDiamondPriceByAverageCarat(sideDiamond.IsLabGrown,sideDiamond.AverageCarat);
            var price = await _diamondServices.GetSideDiamondPrice(sideDiamond);
            jewelry.IsAllSideDiamondPriceKnown = true;
            return sideDiamond.TotalPrice;
        }
        public async Task<Discount?> AssignJewelryDiscount(Jewelry jewelry, List<Discount> discounts)
        {
            return await AssignJewelryDiscountGlobal(jewelry, discounts);
        }
        public static async Task<Discount?> AssignJewelryDiscountGlobal(Jewelry jewelry, List<Discount> discounts)
        {
            Discount mostValuableDiscont = null;
            foreach (var discount in discounts)
            {
                var requirements = discount.DiscountReq;
                foreach (var req in requirements)
                {
                    if (req.TargetType != TargetType.Jewelry_Model)
                        continue;
                    if (req.ModelId != jewelry.ModelId)
                        continue;
                    if (mostValuableDiscont == null)
                    {
                        mostValuableDiscont = discount;
                        break;
                    }
                    else
                    {
                        if (discount.DiscountPercent >= mostValuableDiscont.DiscountPercent)
                        {
                            mostValuableDiscont = discount;
                            break;
                        }
                    }
                }
            }
            if (mostValuableDiscont != null)
            {
                //var reducedAmount = jewelry.TotalPrice * ((decimal)mostValuableDiscont.DiscountPercent / (decimal)100);
                jewelry.AssignJewelryDiscount(mostValuableDiscont);//,reducedAmount
            }
            return mostValuableDiscont;
        }
        public IQueryable<Jewelry> GetJewelryQueryFromModel(JewelryModelId modelId, MetalId metalId, SizeId sizeId, SideDiamondOpt? sideDiamondOpt)
        {
            var jewelryQuery = _jewelryRepository.GetQuery();
            jewelryQuery = _jewelryRepository.QueryInclude(jewelryQuery, p => p.Diamonds);
            jewelryQuery = _jewelryRepository.QueryFilter(jewelryQuery,
                p => p.Status == Common.Enums.ProductStatus.Active &&
                p.MetalId == metalId && p.ModelId == modelId && p.SizeId == sizeId
                );
            if (sideDiamondOpt != null)
                jewelryQuery = jewelryQuery.Where(p => p.SideDiamond.Carat == sideDiamondOpt.CaratWeight
                && p.SideDiamond.ClarityMin == sideDiamondOpt.ClarityMin
                && p.SideDiamond.ClarityMax == sideDiamondOpt.ClarityMax
                && p.SideDiamond.ColorMin == sideDiamondOpt.ColorMin
                && p.SideDiamond.ColorMax == sideDiamondOpt.ColorMax
                && p.SideDiamond.SettingType == sideDiamondOpt.SettingType
                && p.SideDiamond.DiamondShapeId == sideDiamondOpt.ShapeId
                && p.SideDiamond.IsLabGrown == sideDiamondOpt.IsLabGrown
                );
            return jewelryQuery;
        }
    }
}
