using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.Diamonds
{
    public interface IDiamondPriceModificationService
    {
        Task UpdateDiamondPriceTemporally(DiamondPriceUpdateRequest request);
        Task RemoveTemporalDiamondPriceUpdate(DiamondCriteriaId? criteriaId, DiamondShapeId? shapeId, bool isLab, bool isSide);
        Task ApplyOnPriceBoard(DiamondPriceBoardDto createdPriceBoard);
    }
    public record DiamondPriceUpdateRequest
    {
        public DiamondShapeId ShapeId { get; set; }
        public DiamondCriteriaId CriteriaId { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice{ get; set; }
        public bool IsLabDiamond { get; set; }
        public bool IsSideDiamond { get; set; }
    }
}
