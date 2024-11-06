using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondFilterLimitDto
    {
        public List<DiamondShapeDto> Shapes { get; set; } = new();
        public LimitFloatDto Carat { get; set; } = new();
        public LimitDto Color { get; set; } = new();
        public LimitDto Clarity { get; set; } = new();
        public LimitDto Cut { get; set; } = new();
        public LimitDto Polish { get; set; } = new();
        public LimitDto Symmetry { get; set; } = new();
        public LimitFloatDto Price { get; set; } = new() 
        {
            Min = 1000,
            Max = 20_000_000_000,
        };
        public class LimitFloatDto
        {
            public float Min { get; set; } = 0;
            public float Max { get; set; } = 0;
        }
        public class LimitDto
        {
            public int Min { get; set; } = 0;
            public int Max { get; set; } = 0;
        }
    }
    
}
