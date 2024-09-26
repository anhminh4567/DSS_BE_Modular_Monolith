using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondShapeRepository : IBaseRepository<DiamondShape>
    {
        Task<bool> IsAnyItemHaveThisShape(DiamondShapeId diamondShapeId, CancellationToken cancellationToken = default);
    }
}
