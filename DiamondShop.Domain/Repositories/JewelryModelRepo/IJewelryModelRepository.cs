using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryModelRepo
{
    public interface IJewelryModelRepository : IBaseRepository<JewelryModel>
    {
        Task<JewelryModel?> GetByIdMinimal(JewelryModelId id);
        IQueryable<JewelryModel> GetSellingModelQuery();
        IQueryable<JewelryModel> IncludeMainDiamondQuery(IQueryable<JewelryModel> query);
        bool IsExistModelCode(string serialName);
    }
}
