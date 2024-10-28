using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
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
    public class DiamondPrice
    {
        public DiamondShapeId ShapeId { get; set; }
        public DiamondShape Shape { get; set; }
        public DiamondCriteriaId CriteriaId { get; set; }
        public DiamondCriteria Criteria { get; set; }
        public bool IsLabDiamond { get; set; }
        public bool IsSideDiamond { get; set; } = false;
        public decimal Price { get; set; }
        [NotMapped]
        public string? ForUnknownPrice { get; set; }


        public static DiamondPrice Create(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId, decimal price, bool isLabPrice)
        {
            if (price <= 0)
                throw new Exception();
            return new DiamondPrice
            {
                ShapeId = diamondShapeId,
                CriteriaId = diamondCriteriaId,
                Price = price,
                IsLabDiamond = isLabPrice,
                IsSideDiamond = false
            };
        }
        public static DiamondPrice CreateUnknownPrice(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId, bool isLab)
        {
            //this is not supposed to be in db, just for assigning
            return new DiamondPrice
            {
                ShapeId = diamondShapeId,
                CriteriaId = diamondCriteriaId,
                Price = 0,
                ForUnknownPrice = "Liên hệ chúng tôi để được tư vấn giá",
                IsLabDiamond = isLab,
                IsSideDiamond = false,
            };
        }
        public static DiamondPrice CreateSideDiamondPrice(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId, decimal price, bool isLabPrice)
        {
            if (price <= 0)
                throw new Exception();
            return new DiamondPrice
            {
                ShapeId = diamondShapeId,
                CriteriaId = diamondCriteriaId,
                Price = price,
                IsLabDiamond = isLabPrice,
                IsSideDiamond =true,
            };
        }
        public void ChangePrice(decimal price)
        {
            if(price <= 1000)
                throw new Exception();
            Price = price;
        }
        private DiamondPrice() { }
    }
}
