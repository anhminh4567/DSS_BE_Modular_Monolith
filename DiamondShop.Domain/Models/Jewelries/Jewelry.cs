using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamondShop.Domain.Models.Jewelries
{
    public class Jewelry : Entity<JewelryId>, IAggregateRoot
    {
        public JewelryModelId ModelId { get; set; }
        public JewelryModel Model { get; set; }
        public SizeId SizeId { get; set; }
        public Size Size { get; set; }
        public MetalId MetalId { get; set; }
        public Metal Metal { get; set; }
        public float Weight { get; set; }
        public string SerialCode { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped] // this is not sold price, this is just price from diamond and jewelry, no discount or promotion is applied
        public decimal TotalPrice
        {
            get
            {
                var inalPrice = 0m;
                if (ND_Price is not null  )
                    inalPrice += ND_Price.Value;
                if(D_Price is not null)
                    inalPrice += D_Price.Value;
                return inalPrice;
            }
        }


        [NotMapped]
        public bool IsAllDiamondPriceKnown { get => !(Diamonds.Any(d => d.IsPriceKnown) == false); }

        public List<Diamond> Diamonds { get; set; } = new();
        public List<JewelrySideDiamond>? SideDiamonds { get; set; } = new();
        public JewelryReviewId? ReviewId { get; set; }
        public JewelryReview? Review { get; set; }

        public Media? Thumbnail { get; set; }

        public decimal? ND_Price { get; set; }
        public decimal? D_Price { get; set; }
        public decimal? SoldPrice { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;

        private Jewelry() { }
        public static Jewelry Create(
            JewelryModelId modelId, SizeId sizeId, MetalId metalId,
            float weight, string serialCode, ProductStatus status, JewelryId givenId = null)
        {
            return new Jewelry()
            {
                Id = givenId is null ? JewelryId.Create() : givenId,
                ModelId = modelId,
                SizeId = sizeId,
                MetalId = metalId,
                Weight = weight,
                SerialCode = serialCode,
                Status = status,
            };
        }
        public void SetSold()
        {
            Status = ProductStatus.Sold;
        }
        public void SetSell()
        {
            Status = ProductStatus.Active;
            ND_Price = 0;
            D_Price = 0;
            SoldPrice = 0;
        }
        public void SetTotalDiamondPrice(decimal totalAllDiamondPrice)
        {
            D_Price = totalAllDiamondPrice;
        }
    }
}
