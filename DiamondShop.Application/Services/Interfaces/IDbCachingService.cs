using DiamondShop.Application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IDbCachingService
    {
        Task SetValue(DbCacheModel modelToSet);
        Task RemoveValue(string key);
        Task<DbCacheModel> Get(string key);
        Task<List<DbCacheModel>> GetAll();
    
    }
}
