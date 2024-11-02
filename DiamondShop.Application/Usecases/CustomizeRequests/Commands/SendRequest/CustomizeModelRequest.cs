using DiamondShop.Domain.Models.Diamonds.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest
{
    public record CustomizeDiamondRequest(string DiamondShapeId, Clarity? clarity, Color? color, Cut? cut, float? caratFrom, float? caratTo, bool? isLabGrown, Polish? polish, Symmetry? symmetry, Girdle? girdle, Culet? culet);
    public record CustomizeModelRequest(string JewelryModelId, string MetalId, string SizeId, string? SideDiamondOptId, string? EngravedText, string? EngravedFont, string? Note, List<CustomizeDiamondRequest>? CustomizeDiamondRequests);
}
