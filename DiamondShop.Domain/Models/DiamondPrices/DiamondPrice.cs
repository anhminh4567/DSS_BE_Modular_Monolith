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
        public decimal Price { get; set; }
        [NotMapped]
        public string ForUnknownPrice { get; set; }
        [NotMapped]
        public decimal TruePrice { get; set; }
        [NotMapped]
        public Discount? Discount { get; set; }
        [NotMapped]
        public decimal? DiscountPrice { get; set; }
        public static DiamondPrice Create(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId, decimal price)
        {
            if (price <= 0)
                throw new Exception();
            return new DiamondPrice
            {
                ShapeId = diamondShapeId,
                CriteriaId = diamondCriteriaId,
                Price = price,
            };
        }
        public static DiamondPrice CreateUnknownPrice(DiamondShapeId diamondShapeId, DiamondCriteriaId diamondCriteriaId)
        {
            //this is not supposed to be in db, just for assigning
            return new DiamondPrice
            {
                ShapeId = diamondShapeId,
                CriteriaId = diamondCriteriaId,
                Price = 0,
                ForUnknownPrice = "Liên hệ chúng tôi để được tư vấn giá"
            };
        }
        private DiamondPrice() { }
    }
}
