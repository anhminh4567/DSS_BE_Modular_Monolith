using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class SizeMetal
    {
        public SizeId SizeId { get; set; }
        public Size Size { get; set; }
        public MetalId MetalId { get; set; }
        public Metal Metal { get; set; }
        public JewelryModelId ModelId { get; set; }
        public JewelryModel Model { get; set; }
        public float Weight { get; set; }
        [NotMapped]
        public decimal Price
        {
            get
            {
                if (Metal != null)
                {
                    return MoneyVndRoundUpRules.RoundAmountFromDecimal((decimal)Weight * Metal.Price);
                }
                return 0;
            }
            set
            {
                value = Price;
            }
        }
        public SizeMetal() { }
        public static SizeMetal Create(JewelryModelId modelId, MetalId metalId, SizeId sizeId, float weight) => new SizeMetal() { ModelId = modelId, MetalId = metalId, SizeId = sizeId, Weight = weight };
    }
}
