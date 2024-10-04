using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

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
        public bool IsAwaiting { get; set; }
        public bool IsSold { get; set; }
        public List<Diamond> Diamonds { get; set; } = new ();
        public List<JewelrySideDiamond>? SideDiamonds { get; set; } = new ();
        public JewelryReviewId? ReviewId { get; set; }
        public JewelryReview? Review { get; set; }

        public Media? Thumbnail { get; set; }
        public Jewelry() { }
    }
}
