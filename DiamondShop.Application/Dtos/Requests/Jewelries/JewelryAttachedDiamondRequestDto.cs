using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.Jewelries
{
    public record JewelryAttachedDiamondRequestDto(DiamondId DiamondId, int position);
}

