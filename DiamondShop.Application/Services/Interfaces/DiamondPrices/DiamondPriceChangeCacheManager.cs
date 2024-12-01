using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.DeleteRange;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.UpdateRange;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.DiamondPrices
{
    public record DiamondPriceUpdateCacheDto(DiamondCriteriaId CriteriaId,DiamondShapeId ShapeId, bool isLab, bool isSide, decimal priceToChange);
    public record DiamondPriceCreateCacheDto(DiamondCriteriaId CriteriaId, DiamondShapeId ShapeId, bool isLab, bool isSide, decimal newPrice);
    public record DiamondPriceDeleteCacheDto(DiamondCriteriaId CriteriaId, DiamondShapeId ShapeId, bool isLab, bool isSide);
    public interface IDiamondPriceChangeCacheManager
    {
        List<DiamondPrice> SetPriceToUpdate(List<DiamondPriceUpdateCacheDto> updateCaches);
        List<DiamondPrice> SetPriceToCreate(List<DiamondPriceCreateCacheDto> updateCaches);
        List<DiamondPrice> SetPriceToRemove(List<DiamondPriceDeleteCacheDto> updateCaches);
        List<DiamondCriteria> AddCriteria(List<CreateCriteriaFromRangeCommand> updateCaches);
        List<DiamondCriteria> UpdateCriteria(List<UpdateDiamondCriteriaRangeCommand> updateCaches); 
        List<DiamondCriteria> DeleteCriteria(List<DeleteCriteriaByRangeCommand> updateCaches); 
    }
}
