using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Diamonds
{
    public record DiamondCriteriaRequestDto
    {
        public Cut? Cut { get; set; }
        public Clarity Clarity { get; set; }
        public Color Color { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        //public bool? isLabGrown { get; set; }
    }
}
