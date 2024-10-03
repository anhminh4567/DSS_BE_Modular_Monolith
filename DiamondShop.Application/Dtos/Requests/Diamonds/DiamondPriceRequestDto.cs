using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Diamonds
{
    public record DiamondPriceRequestDto(DiamondCriteriaId DiamondCriteriaId, DiamondShapeId DiamondShapeId, decimal price);
    
}
