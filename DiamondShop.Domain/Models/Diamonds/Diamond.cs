using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Common.Products;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.CustomizeRequests;
using DiamondShop.Domain.Models.CustomizeRequests.Entities;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds
{ 
    public record Diamond_4C (Cut? Cut , Color Color, Clarity Clarity, float Carat, bool isLabDiamond);
    public record Diamond_Measurement(float withLenghtRatio, float Depth, float table, string Measurement);
    public record Diamond_Details( Polish Polish, Symmetry Symmetry, Girdle Girdle, Fluorescence Fluorescence, Culet Culet);

    public class Diamond : Entity<DiamondId> , IAggregateRoot
    {  
        public static ProductStatus[] UnallowedToDeleteStatus = new ProductStatus[] { ProductStatus.Sold, ProductStatus.Locked, ProductStatus.PreOrder };
        public static ProductStatus[] UnallowedToAssignStatus = new ProductStatus[] { ProductStatus.Sold, ProductStatus.Locked, ProductStatus.PreOrder, };

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
        public string? CertificateCode { get; set; }
        public Media? CertificateFilePath { get; set; }
        public string Measurement { get; set; }
        public Media? Thumbnail { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public ProductLock? ProductLock { get; set; }
        public decimal? SoldPrice { get; set; }
        public decimal? DefaultPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public decimal? FixPriceOffset { get; set; }
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
        [NotMapped]
        public decimal? CalculatedPrice { get; set; }
        [NotMapped]
        public DiamondRequest? DiamondRequest { get; set; }
        [NotMapped]
        public Jewelry? Jewelry { get; set; }
        public static Diamond Create(DiamondShape shape, Diamond_4C diamond_4C, Diamond_Details diamond_Details,
           Diamond_Measurement diamond_Measurement,decimal priceOffset,string? sku, Certificate certificate = Certificate.GIA) 
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = ProductStatus.Active
            };
            if(sku!= null)
                newdiamond.SerialCode = sku;
            else
                newdiamond.SerialCode = DiamondRule.GetDiamondSerialCode(newdiamond,shape);
            return newdiamond;
        }
        public void UpdatePriceOffset(decimal priceOffset)
        {
            PriceOffset = priceOffset;
        }
        public void SetForJewelry (Jewelry jewelry, bool isRemove = false) 
        {
            if (isRemove)
            {
                SetSell();// set sell to clean up the price
                JewelryId = null;//if remove a diamond from jewelry it will go to inactive 
                Status = ProductStatus.Inactive;
            }
            else
            {
                var currentPrice = DefaultPrice;
                //SetSell();
                if(Status == ProductStatus.PreOrder)
                {
                    JewelryId = jewelry.Id;
                    return;
                }
                if(jewelry.Status != ProductStatus.PreOrder && jewelry.Status != ProductStatus.Sold)
                    SetSell();
                JewelryId = jewelry.Id;
                Status = ProductStatus.Locked;
            } 
        }
        public static string GetTitle(Diamond diamond)
        {
            var shapeId = diamond.DiamondShapeId;
            var getShape = DiamondShape.All_Shape.First(x => x.Id == shapeId);
            var baseString = $"{diamond.Carat} carat {diamond.Color.ToString()}-{diamond.Clarity.ToString()} ";
            if(diamond.Cut != null)
            {
                baseString = $"{baseString} {diamond.Cut.ToString()} Cut ";
            }
            baseString = $"{baseString} {getShape.Shape} Shape";
            string origin = diamond.IsLabDiamond ? " Lab" : " Natural";
            baseString = $"{baseString} {origin} diamond";
            return baseString;
        }
        public static string GetDescription(Diamond diamond)
        {
            return GetTitle(diamond);
        }
        public void DetachJewelry()
        {
            JewelryId = null;
            SetSell();
        }
        public void SetSold(decimal defaultPrice ,decimal soldPrice)
        {
            Status = ProductStatus.Sold;
            DefaultPrice = defaultPrice;
            SoldPrice = soldPrice;
            ProductLock = null;
        }
        public void SetSoldPreOrder(decimal defaultPrice, decimal soldPrice)
        {
            Status = ProductStatus.PreOrder;
            DefaultPrice = defaultPrice;
            SoldPrice = soldPrice;
            ProductLock = null;
        }
        public void SetSell()
        {
            //if (Status == ProductStatus.Sold)
            //    throw new Exception("Cannot change status of a sold item");
            //if (JewelryId != null)
            //    throw new Exception("Cannot change status of this diamond, since it is attached to a jewelry already");
            Status = ProductStatus.Active;
            ProductLock = null;
            SoldPrice = null;
            DefaultPrice = null;
            ProductLock = null;
            JewelryId = null;
        }
        public void SetLockForCustomizeRequest()
        {
            Status = ProductStatus.Locked;
            SoldPrice = null;
            DefaultPrice = null;
            ProductLock = null;
        }
        public void SetInActive()
        {
            if(Status == ProductStatus.Sold)
                throw new Exception("sản phẩm đã bán, không làm được hành động nào");
            if (JewelryId != null)
                throw new Exception("không đổi trạng thái được, do đã gắn với 1 trang sức, cần xóa trang sức");
            Status = ProductStatus.Inactive;
            SoldPrice = null;
            DefaultPrice = null;
            ProductLock = null;
            //UpdatedAt = DateTime.UtcNow;
        }   
        public void SetCorrectPrice(decimal truePrice, DiamondRule rule)
        {
            if(DiamondPrice != null && DiamondPrice.ForUnknownPrice != null)
            {
                TruePrice = 0;
                return;
            }    
            var priceAfterCarat = truePrice * (decimal)Carat;
            decimal correctOffset = 1 + PriceOffset;
            var priceAfterOffset = priceAfterCarat * correctOffset;//MoneyVndRoundUpRules.RoundAmountFromDecimal(
            var truePriceAfterCheckingMinDiamondPriceRule = Math.Clamp(priceAfterOffset, rule.MinimalMainDiamondPrice, decimal.MaxValue);
            var finalPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(truePriceAfterCheckingMinDiamondPriceRule);
            if (TruePrice < 0)
                throw new Exception();
            else
            {
                if (FixPriceOffset != null)
                    TruePrice = finalPrice + FixPriceOffset.Value;
                else
                    TruePrice = finalPrice;
            }
        }
        public void SetLockPriceForJewelry(int lockHour, decimal? LockedPriceForCustomer, DiamondRule rule)
        {
            if (Status == ProductStatus.Sold)
                throw new Exception("sản phẩm đã bán");
            if(LockedPriceForCustomer < rule.MinimalMainDiamondPrice)
                throw new Exception("giá bán phải lớn hơn giá tối thiểu là "+ rule.MinimalMainDiamondPrice);
            if (lockHour > rule.MaxLockTimeForCustomer || lockHour < 1)
                throw new Exception("thoi gian lock san pham toi da la  " + rule.MaxLockTimeForCustomer + " va toi thieu la 1");
            if (JewelryId == null || Status != ProductStatus.Locked)
                throw new Exception("không thể khóa giá cho kim cương này, chỉ khi khóa cho trang sức, còn TH còn lại không được khóa gía");
            Status = ProductStatus.LockForUser;
            ProductLock = ProductLock.CreateLock( TimeSpan.FromHours(lockHour));
            if (LockedPriceForCustomer != null)
            {
                DefaultPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(LockedPriceForCustomer.Value);
            }
            else
            {
                DefaultPrice = null;
            }
        }
        public void SetLockForUser(Account userAccount , int lockHour, decimal? LockedPriceForCustomer, DiamondRule rule)
        {
            if (Status == ProductStatus.Sold)
                throw new Exception("sản phẩm đã bán");
            if (LockedPriceForCustomer < rule.MinimalMainDiamondPrice)
                throw new Exception("giá bán phải lớn hơn giá tối thiểu là " + rule.MinimalMainDiamondPrice);
            if (lockHour > rule.MaxLockTimeForCustomer || lockHour < 1)
                throw new Exception("thoi gian lock san pham toi da la  " + rule.MaxLockTimeForCustomer + " va toi thieu la 1");
            if (Status != ProductStatus.Active)
                throw new Exception("chỉ sản phẩm đang mở bán mới khóa được cho người dùng");
            Status = ProductStatus.LockForUser;
            ProductLock = ProductLock.CreateLockForUser(userAccount.Id, TimeSpan.FromHours(lockHour));
            if(LockedPriceForCustomer != null)
            {
                DefaultPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(LockedPriceForCustomer.Value);
            }
            else
            {
                DefaultPrice = null;
            }
                
        }
        public void PreOrder(decimal? askingPrice)
        {
            if (Status == ProductStatus.Sold)
                throw new Exception("sản phẩm đã bán");
            if (Status == ProductStatus.Locked)
                throw new Exception("sản phẩm bị khóa, ko pre-order");
            Status = ProductStatus.PreOrder;
            if(askingPrice < 0)
                throw new Exception("Giá của món hàng phải lớn hay bằng 0, nếu = 0 thì miễn phí");
            if (askingPrice != null)
                DefaultPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(askingPrice.Value);
        }
        public void RemoveLock()
        {
            if (Status == ProductStatus.Sold)
                throw new Exception("cannot unlock an already sold item");
            if(JewelryId != null)
            {
                ProductLock = null;
                SoldPrice = null;
                DefaultPrice = null;
                Status = ProductStatus.Locked;
                //UpdatedAt = DateTime.UtcNow;
                return;
            }
            SetSell();
            //UpdatedAt = DateTime.UtcNow;
        }
        public void ChangeThumbnail(Media? thumbnail)
        {
            Thumbnail = thumbnail;
            //UpdatedAt = DateTime.UtcNow;
        }
        public void ChangeOffset(decimal newOffset)
        {
            PriceOffset = newOffset;
            UpdatedAt = DateTime.UtcNow;
        }
        public void SetExtraFee(decimal? extraFee)
        {
            if (extraFee != null)
                if (extraFee < 0)
                    throw new Exception();
                else
                    FixPriceOffset = extraFee;
            else
                FixPriceOffset = null;
            UpdatedAt = DateTime.UtcNow;
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
        public void SetCertificate(string certificateCode, Media certificateFile)
        {
            CertificateCode = certificateCode;
            CertificateFilePath = certificateFile;
        }
        public void SetActive(bool isActive)
        {
            if(Status == ProductStatus.Sold)
                throw new Exception("Cannot change status of a sold item");
            if(Status == ProductStatus.Locked)
                throw new Exception("Cannot change status of a locked diammond for jewelry");
            if (Status == ProductStatus.PreOrder)
                throw new Exception("Cannot change status of a pre-order diamond");
            if (Status == ProductStatus.LockForUser)
                throw new Exception("Cannot change status of a locked diamond for user");
            if (isActive)
                SetSell();
            else
                SetInActive();
                //Status = ProductStatus.Inactive;
        }
        public bool IsSetForCustomizeJewelryNotExistingInShop()
        {
            if (JewelryId != null && Status == ProductStatus.PreOrder)
                return true ;
            if (Status == ProductStatus.PreOrder)
                return true;
            return false;
        }
        public void DealdedPrice(decimal dealedPrice)
        {
            if (Status != ProductStatus.PreOrder)
                throw new Exception("chỉ được tự deal giá nếu kim cương trong pre order");
            DefaultPrice = dealedPrice;
        }
        public Diamond() { }
    }
}
