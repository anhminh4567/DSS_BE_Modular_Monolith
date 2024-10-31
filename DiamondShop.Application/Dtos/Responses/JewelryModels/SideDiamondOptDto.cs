using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class SideDiamondOptDto
    {
        public string Id { get; set; }
        public string ModelId { get; set; }
        public string ShapeId { get; set; }
        public DiamondShapeDto Shape { get; set; }
        public string ColorMin { get; set; }
        public string ColorMax { get; set; }
        public string ClarityMin { get; set; }
        public string ClarityMax { get; set; }
        public string SettingType { get; set; }
        public float CaratWeight { get; set; }
        public int Quantity { get; set; }
    }
}
