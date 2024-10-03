using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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
        public SizeMetal() { }
        public static SizeMetal Create(JewelryModelId modelId, MetalId metalId, SizeId sizeId, float weight) => new SizeMetal() { ModelId = modelId, MetalId = metalId, SizeId = sizeId, Weight = weight };
    }
}
