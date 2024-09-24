using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.Diamonds
{
    public class DiamondCriteriaDto
    {
        public string Id { get; set; }
        public Cut Cut { get; set; }
        public Clarity Clarity { get; set; }
        public Color Color { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
    }
}
