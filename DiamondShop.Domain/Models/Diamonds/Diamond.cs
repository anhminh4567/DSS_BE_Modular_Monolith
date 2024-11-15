using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Common.Products;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds
{ 
    public record Diamond_4C (Cut? Cut , Color Color, Clarity Clarity, float Carat, bool isLabDiamond);
    public record Diamond_Measurement(float withLenghtRatio, float Depth, float table, string Measurement);
    public record Diamond_Details( Polish Polish, Symmetry Symmetry, Girdle Girdle, Fluorescence Fluorescence, Culet Culet);
    public class Diamond : Entity<DiamondId> , IAggregateRoot
    {  
        public JewelryId? JewelryId { get;  set; }
        public DiamondShapeId DiamondShapeId { get; set;}
        public DiamondShape DiamondShape { get; set;}
        //public DiamondWarranty? Warranty { get; set;}
        /*public List<DiamondMedia> Medias { get; set;} = new();*/
        public Clarity Clarity { get; set;}
        public Color Color { get; set;}
        public Cut? Cut { get; set;}
        public decimal PriceOffset { get; set; } = 0;
        public float Carat { get; set; } 
        public bool IsLabDiamond { get; set;}
        public float WidthLengthRatio { get; set; }
        public float Depth { get; set; }
        public float Table { get; set; }
        public string? SerialCode { get; set; }
        public Polish Polish { get; set; }
        public Symmetry Symmetry { get; set; }
        public Girdle Girdle { get; set; }
        public Culet Culet { get; set; }
        public Fluorescence Fluorescence { get; set; }
        public Certificate Certificate { get; set; } = Certificate.GIA;
        //public string? CertificateCode { get; set; }
        //public Media? CertificateFile { get; set; }
        public string Measurement { get; set; }
        public Media? Thumbnail { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public ProductLock? ProductLock { get; set; }
        public decimal? SoldPrice { get; set; }
        public decimal? DefaultPrice { get; set; }
        [NotMapped]
        public bool IsSetForJewelry { get => JewelryId != null; }
        [NotMapped]
        public DiamondPrice? DiamondPrice { get; set; }
        [NotMapped]
        public decimal TruePrice { get; set; } = 0;
        [NotMapped]
        public bool IsPriceKnown { get =>  TruePrice > 0 ; }
        [NotMapped]
        public Discount? Discount { get; set; }
        [NotMapped]
        public decimal? DiscountPrice { get; set; }
        [NotMapped]
        public decimal DiscountReducedAmount { get; set; } = 0;
        //[NotMapped]
        //public Promotion? Promotion { get; set; }
        [NotMapped]
        public decimal PromotionReducedAmount { get; set; } = 0;
        [NotMapped]
        public decimal? SalePrice { get => Math.Clamp(
                MoneyVndRoundUpRules.RoundAmountFromDecimal(TruePrice - DiscountReducedAmount - PromotionReducedAmount),0,decimal.MaxValue); }
        
        [NotMapped]
        public string Title { get => GetTitle(this); }
        [NotMapped]
        public decimal CutOffsetFounded { get; set; }
        public static Diamond Create(DiamondShape shape, Diamond_4C diamond_4C, Diamond_Details diamond_Details,
           Diamond_Measurement diamond_Measurement,decimal priceOffset, Certificate certificate = Certificate.GIA) 
        {
            var newdiamond =  new Diamond()
            {
                Id = DiamondId.Create(),
                DiamondShapeId = shape.Id,
                Clarity = diamond_4C.Clarity,
                Color = diamond_4C.Color,
                Cut = diamond_4C.Cut,
                Carat = diamond_4C.Carat,
                IsLabDiamond = diamond_4C.isLabDiamond,
                WidthLengthRatio = diamond_Measurement.withLenghtRatio,
                Depth = diamond_Measurement.Depth,
                Table = diamond_Measurement.Depth,
                Polish = diamond_Details.Polish,
                Symmetry = diamond_Details.Symmetry,
                Girdle = diamond_Details.Girdle,
                Culet = diamond_Details.Culet,
                Fluorescence = diamond_Details.Fluorescence,
                Measurement = diamond_Measurement.Measurement,
                PriceOffset = priceOffset,
                SoldPrice = null,
                DefaultPrice = null,
                Certificate = certificate,
            };
            newdiamond.SerialCode = DiamondRule.GetDiamondSerialCode(newdiamond,shape);
            return newdiamond;
        }
        public void UpdatePriceOffset(decimal priceOffset)
        {
            PriceOffset = priceOffset;
        }
        public void SetForJewelry (Jewelry jewelry, bool isRemove = false) 
        {
            if(isRemove)
                JewelryId = null;
            else 
                JewelryId = jewelry.Id; 
        }
        public static string GetTitle(Diamond diamond)
        {
            var shapeId = diamond.DiamondShapeId;
            var getShape = DiamondShape.All_Shape.First(x => x.Id == shapeId);
            return $"{diamond.Carat} carat {diamond.Color.ToString()}-{diamond.Clarity.ToString()} {diamond.Cut.ToString()} Cut {getShape.Shape} diamond";
        }
        public static string GetDescription(Diamond diamond)
        {
            return GetTitle(diamond);
        }
        public void SetSold(decimal defaultPrice ,decimal soldPrice)
        {
            Status = ProductStatus.Sold;
            DefaultPrice = defaultPrice;
            SoldPrice = soldPrice;
            ProductLock = null;
        }
        public void SetSell()
        {
            Status = ProductStatus.Active;
            ProductLock = null;
            SoldPrice = null;
            DefaultPrice = null;
            ProductLock = null;
        }
        public void SetDeactivate()
        {
            Status = ProductStatus.Locked;
            SoldPrice = null;
            DefaultPrice = null;
            ProductLock = null;
        }
        public void SetCorrectPrice(decimal truePrice, DiamondRule rulesToSetCutOffSet)
        {
            var priceAfterCarat = truePrice * (decimal)Carat;
            decimal correctOffset = 1 + PriceOffset;
            var priceAfterOffset = MoneyVndRoundUpRules.RoundAmountFromDecimal(priceAfterCarat * correctOffset);
            if (TruePrice < 0)
                throw new Exception();
            else
                TruePrice = priceAfterOffset;
        }
        public void SetLockForUser(Account userAccount , int lockHour)
        {
            if (Status == ProductStatus.Sold)
                throw new Exception("cannot lock an already sold item");
            Status = ProductStatus.Locked;
            ProductLock = ProductLock.CreateLockForUser(userAccount.Id, TimeSpan.FromHours(lockHour));
        }
        public void RemoveLock()
        {
            if (Status == ProductStatus.Sold)
                throw new Exception("cannot unlock an already sold item");
            Status = ProductStatus.Active;
            ProductLock = null;
        }
        public void ChangeThumbnail(Media? thumbnail) => Thumbnail = thumbnail;
        public void ChangeOffset(decimal newOffset)
        {
            if (newOffset <= 0)
                throw new Exception();
            PriceOffset = newOffset;
        }
        public void AssignPromotion(Promotion promotion, decimal reducedAmount)
        {
            //Promotion = promotion;
            PromotionReducedAmount = reducedAmount;
        }
        public void AssignDiscount(Discount discount, decimal reducedAmount)
        {
            //Promotion = promotion;
            DiscountReducedAmount = reducedAmount;
            Discount = discount;
        }
        private Diamond() { }
    }
}
