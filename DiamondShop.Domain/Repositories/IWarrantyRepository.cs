using DiamondShop.Domain.Models.Warranties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IWarrantyRepository : IBaseRepository<Warranty>
    {
        bool IsCodeExist(string code);
        bool IsNameExist(string name);
    }
}
