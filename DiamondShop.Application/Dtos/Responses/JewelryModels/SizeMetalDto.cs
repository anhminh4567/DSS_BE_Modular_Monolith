using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class SizeMetalDto
    {
        public SizeDto Size { get; set; }
        public MetalDto Metal { get; set; }
        public JewelryModelDto Model { get; set; }
        public float Weight { get; set; }
    }
}
