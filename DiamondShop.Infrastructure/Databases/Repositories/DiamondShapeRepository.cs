using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class DiamondShapeRepository : BaseRepository<DiamondShape>, IDiamondShapeRepository
    {
        public DiamondShapeRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }

        public Task<List<DiamondShape>> GetAllIncludeSpecialShape(CancellationToken cancellationToken = default)
        {
            return _set.IgnoreQueryFilters().ToListAsync(cancellationToken);
        }

        public override async Task<DiamondShape?> GetById(params object[] ids)
        {
            DiamondShapeId id = (DiamondShapeId)ids[0];
            return await _set.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<bool> IsAnyItemHaveThisShape(DiamondShapeId diamondShapeId, CancellationToken cancellationToken = default)
        {
            var anyPrice = _dbContext.DiamondCriteria.AnyAsync(p => p.ShapeId.Equals(diamondShapeId), cancellationToken);
            var anyDiamond = _dbContext.MainDiamondShapes.AnyAsync(p => p.ShapeId.Equals(diamondShapeId), cancellationToken);
            var anySideDiamond = _dbContext.SideDiamondOpts.AnyAsync(p => p.ShapeId.Equals(diamondShapeId), cancellationToken);
            var anyPromoShape = _dbContext.PromoReqShapes.AnyAsync(p => p.ShapeId.Equals(diamondShapeId), cancellationToken);
            var results =await Task.WhenAll(anyPrice, anyDiamond, anySideDiamond, anyPromoShape);
            foreach(var isOk in results) 
            {
                if (isOk is false)
                    return false;
                continue;
            }
            return true;
        }
    }
}
