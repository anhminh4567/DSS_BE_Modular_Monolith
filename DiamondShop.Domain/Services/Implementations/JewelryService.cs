using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common.Carts;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class JewelryService : IJewelryService
    {
        private readonly IJewelrySideDiamondRepository _jewelrySideDiamondRepository;
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondServices _diamondServices;
        private readonly IDiamondPriceRepository _diamondPriceRepository;
        private readonly IDiscountRepository _discountRepository;

        public JewelryService(IJewelrySideDiamondRepository jewelrySideDiamondRepository, IDiamondRepository diamondRepository, IDiamondServices diamondServices, IDiamondPriceRepository diamondPriceRepository, IDiscountRepository discountRepository)
        {
            _jewelrySideDiamondRepository = jewelrySideDiamondRepository;
            _diamondRepository = diamondRepository;
            _diamondServices = diamondServices;
            _diamondPriceRepository = diamondPriceRepository;
            _discountRepository = discountRepository;
        }

        public bool SetupUnmapped(List<Jewelry> jewelries, List<SizeMetal> sizeMetals)
        {
            foreach (var jewelry in jewelries)
            {
                if (jewelry.Metal == null) return false;
                StringBuilder sb = new StringBuilder();
                sb.Append($"{jewelry.Metal.Name[0]}-{jewelry.Model.Name}");
                if (jewelry.SideDiamonds != null)
                {
                    string[] arr = new string[jewelry.SideDiamonds.Count];
                    jewelry.SideDiamonds.ForEach(d => arr.Append($"Avg.{d.Carat}"));
                    sb.Append(String.Join("|", arr));
                }
                jewelry.Name = sb.ToString();
                var sizeMetal = sizeMetals.FirstOrDefault(k => jewelry.ModelId == k.ModelId && jewelry.SizeId == k.SizeId && jewelry.MetalId == k.MetalId);
                if (sizeMetal?.Metal == null)
                    return false;
                jewelry.ND_Price = sizeMetal.Price != null ? sizeMetal.Price : GetPrice(sizeMetal.Weight, jewelry.Metal.Price);
            }
            return true;
        }
        public Jewelry AddPrice(Jewelry jewelry, ISizeMetalRepository sizeMetalRepository)
        {
            var query = sizeMetalRepository.GetQuery();
            query = sizeMetalRepository.QueryInclude(query, p => p.Metal);
            query = sizeMetalRepository.QueryFilter(query, p => p.ModelId == jewelry.ModelId && p.SizeId == jewelry.SizeId && p.MetalId == jewelry.MetalId);
            var sizeMetal = query.FirstOrDefault();
            if (sizeMetal != null)
            {
                jewelry.ND_Price = GetPrice(sizeMetal.Weight, sizeMetal.Metal.Price);
            }
            return jewelry;
        }

        public Jewelry AddPrice(Jewelry jewelry, SizeMetal sizeMetal)
        {
            if (sizeMetal.Metal == null) return null;
            jewelry.ND_Price = GetPrice(sizeMetal.Weight, sizeMetal.Metal.Price);
            return jewelry;
        }
        private decimal GetPrice(float Weight, decimal Price) => (decimal)Weight * Price;

        private async Task<decimal> GetJewelryDiamondPrice(Jewelry jewelry) 
        {
            var getJewelryDiamonds = await _diamondRepository.GetDiamondsJewelry(jewelry.Id);
            jewelry.Diamonds = getJewelryDiamonds;
            decimal D_price= 0;
            foreach (var diamond in getJewelryDiamonds)
            {
                var prices = await _diamondPriceRepository.GetPriceByShapes(diamond.DiamondShape, diamond.IsLabDiamond);
                var diamondPrice = _diamondServices.GetDiamondPrice(diamond, prices).Result;
                diamond.DiamondPrice = diamondPrice;
            }
            D_price = jewelry.Diamonds.Sum(d => d.TruePrice);
            jewelry.D_Price = D_price;
            return D_price;
        }
        private async Task<decimal> GetJewelrySideDiamondPrice(Jewelry jewelry)
        {
            throw new NotImplementedException();
        }

        public async Task<Discount?> AssignJewelryDiscount(Jewelry jewelry, List<Discount> discounts)
        {
            Discount mostValuableDiscont = null;
            foreach(var discount in discounts)
            {
                var requirements = discount.DiscountReq;
                foreach(var req in requirements)
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
                        if(discount.DiscountPercent >= mostValuableDiscont.DiscountPercent)
                        {
                            mostValuableDiscont = discount;
                            break;
                        }
                    }
                }
            }
            return mostValuableDiscont;
        }
    }
}
