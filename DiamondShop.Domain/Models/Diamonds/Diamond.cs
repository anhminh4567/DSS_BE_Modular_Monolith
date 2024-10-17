using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds
{ 
    public record Diamond_4C (Cut Cut , Color Color, Clarity Clarity, float Carat, bool isLabDiamond);
    public record Diamond_Measurement(float withLenghtRatio, float Depth, float table, string Measurement);
    public record Diamond_Details( Polish Polish, Symmetry Symmetry, Girdle Girdle, Fluorescence Fluorescence, Culet Culet);
    public class Diamond : Entity<DiamondId> , IAggregateRoot
    {  
        public JewelryId? JewelryId { get;  set; }
        public DiamondShapeId DiamondShapeId { get; set;}
        public DiamondShape DiamondShape { get; set;}
      //  public DiamondWarranty? Warranty { get; set;}
        /*public List<DiamondMedia> Medias { get; set;} = new();*/
        public Clarity Clarity { get; set;}
        public Color Color { get; set;}
        public Cut? Cut { get; set;}
        public decimal PriceOffset { get; set;}
        public float Carat { get; set; } 
        public bool IsLabDiamond { get; set;}
        public float WidthLengthRatio { get; set; }
        public float Depth { get; set; }
        public float Table { get; set; }
        public Polish Polish { get; set; }
        public Symmetry Symmetry { get; set; }
        public Girdle Girdle { get; set; }
        public Culet Culet { get; set; }
        public Fluorescence Fluorescence { get; set; }
        public string Measurement { get; set; }
        public Media? Thumbnail { get; set; }
        public List<Media>? Gallery { get; set; } = new();
        public bool IsSold { get; set; } = false;
        public bool IsActive { get; set; } = true;
        [NotMapped]
        public DiamondPrice? DiamondPrice { get; set; }
        public static Diamond Create(DiamondShape shape, Diamond_4C diamond_4C, Diamond_Details diamond_Details,
           Diamond_Measurement diamond_Measurement,decimal priceOffset) 
        {
            return new Diamond()
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
                PriceOffset = Math.Clamp(priceOffset,DiamondRules.MinPriceOffset,DiamondRules.MaxPriceOffset),
            };
        }
        public void UpdatePriceOffset(decimal priceOffset)
        {
            PriceOffset = Math.Clamp(priceOffset, DiamondRules.MinPriceOffset, DiamondRules.MaxPriceOffset);
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
            return $"{diamond.Carat} carat {diamond.Color.ToString()}-{diamond.Clarity.ToString()} {diamond.Cut.ToString()} Cut {diamond.DiamondShape.Shape} diamond";
        }
        public static string GetDescription(Diamond diamond)
        {
            return GetTitle(diamond);
        }
        public void SetSold()
        {
            IsActive = false;
            IsSold = true;
        }
        public void SetSell()
        {
            IsActive = true;
            IsSold = false;
        }
        public void ChangeThumbnail(Media? thumbnail) => Thumbnail = thumbnail;
        private Diamond() { }
    }
}
