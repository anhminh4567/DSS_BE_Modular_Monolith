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
        public override Task<DiamondShape?> GetById(params object[] ids)
        {
            DiamondShapeId id = (DiamondShapeId)ids[0];
            return _set.Include(d => d.Shape).FirstOrDefaultAsync(s => s.Id.Value == id.Value);
        }
        public async Task<bool> IsAnyItemHaveThisShape(DiamondShapeId diamondShapeId)
        {
            var anyPrice = _dbContext.DiamondPrices.AnyAsync(p => p.ShapeId.Equals(diamondShapeId));
            var anyDiamond = _dbContext.MainDiamondShapes.AnyAsync(p => p.ShapeId.Equals(diamondShapeId));
            var anySideDiamond = _dbContext.SideDiamondReqs.AnyAsync(p => p.ShapeId.Equals(diamondShapeId));
            var anyPromoShape = _dbContext.PromoReqShapes.AnyAsync(p => p.ShapeId.Equals(diamondShapeId));
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
