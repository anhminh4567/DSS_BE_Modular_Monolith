using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class SizeMetalDto
    {
        public string SizeId { get; set; }
        public string MetalId { get; set; }
        public SizeDto Size { get; set; }
        public MetalDto Metal { get; set; }
        public float Weight { get; set; }
    }
}
