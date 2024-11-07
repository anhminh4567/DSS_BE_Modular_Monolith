using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using DiamondShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories
{
    internal class WarrantyRepository : BaseRepository<Warranty>, IWarrantyRepository
    {
        public WarrantyRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {
        }
        public override Task<Warranty?> GetById(params object[] ids)
        {
            var id = (WarrantyId)ids[0];
            return _set.FirstOrDefaultAsync(p => p.Id == id);
        }
        public bool IsCodeExist(string code)
        {
            return _set.Any(p => p.Code.ToUpper() ==  code.ToUpper());
        }
        public bool IsNameExist(string name)
        {
            return _set.Any(p => p.Name.ToUpper() == name.ToUpper());
        }
    }
}
