using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices
{
    public class DiamondPrice : Entity<DiamondPriceId>
    {
        //public DiamondShapeId ShapeId { get; set; }
        //public DiamondShape Shape { get; set; }
        public static DiamondPriceId UknonwDiamondPrice = DiamondPriceId.Parse("-1");
        public DiamondCriteriaId CriteriaId { get; set; }
        public DiamondCriteria Criteria { get; set; }
        public bool IsLabDiamond { get; set; }
        public bool IsSideDiamond { get; set; } = false;
        public Cut? Cut { get; set; }
        public Clarity? Clarity { get; set; }
        public Color? Color { get; set; }
        public AccountId? AccountId { get; set; }
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public decimal? OldPrice { get; set; }
        public DateTime? OldUpdatedAt { get; set; }
        public AccountId? OldAccountIdUpdate { get; set; }
        [NotMapped]
        public string? ForUnknownPrice { get; set; }

        //[NotMapped]
        //public bool IsFancyShape => ShapeId == DiamondShape.FANCY_SHAPES.Id;
        //public const bool DEFAULT_SIDE_DIAMOND_IS_LAB = true;
        public static DiamondPrice Create(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId, decimal price, bool isLabPrice,Cut? cut, Color color, Clarity clarity)
        {
            if (price <= 0)
                throw new Exception();
            return new DiamondPrice
            {
                Id = DiamondPriceId.Create(),
                //ShapeId = diamondShapeId,
                CriteriaId = diamondCriteriaId,
                Price = price,
                IsLabDiamond = isLabPrice,
                IsSideDiamond = false,
                Clarity = clarity,
                Color = color,
                Cut = null,
            };
        }
        public static DiamondPrice CreateUnknownPrice(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId, bool isLab)
        {
            //this is not supposed to be in db, just for assigning
            return new DiamondPrice
            {
                //ShapeId = diamondShapeId,
                Id = UknonwDiamondPrice,
                CriteriaId = diamondCriteriaId,
                Price = 0,
                ForUnknownPrice = "Liên hệ chúng tôi để được tư vấn giá",
                IsLabDiamond = isLab,
                IsSideDiamond = false,
                Clarity = null,
                Color = null,
                Cut = null,
            };
        }
        public static DiamondPrice CreateSoldPrice(Diamond soldDiamond)
        {
            //this is not supposed to be in db, just for assigning
            return new DiamondPrice
            {
                //ShapeId = diamondShapeId,
                Id = DiamondPriceId.Parse("-11"),
                CriteriaId = null,
                Price = soldDiamond.SoldPrice ?? 0,
                ForUnknownPrice = "Đã bán",
                IsLabDiamond = soldDiamond.IsLabDiamond,
                IsSideDiamond = false,
                Clarity = soldDiamond.Clarity,
                Color = soldDiamond.Color,
                Cut = soldDiamond.Cut,
            };
        }
        public static DiamondPrice CreateDealedLockedPriceForUser(Diamond lockedDiamond)
        {
            //this is not supposed to be in db, just for assigning
            return new DiamondPrice
            {
                //ShapeId = lockedDiamond.DiamondShapeId,
                Id = DiamondPriceId.Create(),
                CriteriaId = null,
                Price = lockedDiamond.DefaultPrice.Value,
                //ForUnknownPrice = "Liên hệ chúng tôi để được tư vấn giá",
                IsLabDiamond = lockedDiamond.IsLabDiamond,
                IsSideDiamond = false,
                Clarity = lockedDiamond.Clarity,
                Color = lockedDiamond.Color,
                Cut = lockedDiamond.Cut,
            };
        }
        public static DiamondPrice CreateUnknownSideDiamondPrice(bool isLab)
        {
            //this is not supposed to be in db, just for assigning
            return new DiamondPrice
            {
                Id = UknonwDiamondPrice,
                //ShapeId = DiamondShape.ANY_SHAPES.Id,
                CriteriaId = DiamondCriteriaId.Parse("-1"),
                Price = 0,
                ForUnknownPrice = "giá kim cương tấm chưa được set rõ",
                IsLabDiamond = isLab,// assume all side diamond is lab
                IsSideDiamond = true,
                Clarity = null,
                Color = null,
                Cut = null,
            };
        }
        public static DiamondPrice CreateSideDiamondPrice( DiamondCriteriaId diamondCriteriaId, decimal price,bool isLabDiamond, DiamondShape shape, Cut? cut, Color color, Clarity clarity)
        {
            if (price <= 0)
                throw new Exception();
            return new DiamondPrice
            {
                Id = DiamondPriceId.Create(),
                //ShapeId = shape.Id, 
                CriteriaId = diamondCriteriaId,
                Price = price,
                IsLabDiamond = isLabDiamond,
                IsSideDiamond =true,
                Clarity = clarity,
                Color = color,
                Cut = cut,
            };
        }
        public void ChangePrice(decimal price,AccountId? whoChangeThePriceId)
        {
            if(price <= 1000)
                throw new Exception();
            OldPrice = Price;
            OldUpdatedAt = UpdatedAt;
            OldAccountIdUpdate = AccountId ;
            Price = price;
            UpdatedAt = DateTime.UtcNow;
            AccountId = whoChangeThePriceId == null ? null : whoChangeThePriceId ;
        }
        private DiamondPrice() { }
    }
}
