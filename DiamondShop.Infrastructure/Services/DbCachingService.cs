using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Services.Models;
using DiamondShop.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services
{
    internal class DbCachingService : IDbCachingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DiamondShopDbContext _dbContext;

        public DbCachingService(IUnitOfWork unitOfWork, DiamondShopDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public Task<DbCacheModel?> Get(string key)
        {
            return _dbContext.DbCacheModels.FirstOrDefaultAsync(x => x.KeyId == key);
        }

        public Task<List<DbCacheModel>> GetAll()
        {
            return _dbContext.DbCacheModels.ToListAsync();
        }

        public Task RemoveValue(string key)
        {
            var getKey = _dbContext.DbCacheModels.FirstOrDefault(x => x.KeyId == key);
            if(getKey != null)
                _dbContext.DbCacheModels.Remove(getKey);
            return _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveValues(string keysContain)
        {
            var getKey = await _dbContext.DbCacheModels.Where(x => x.KeyId.Contains(keysContain)).ToListAsync();
            if (getKey != null)
                _dbContext.DbCacheModels.RemoveRange(getKey);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task SetValue(DbCacheModel modelToSet)
        {
            var getKey = _dbContext.DbCacheModels.FirstOrDefault(x => x.KeyId == modelToSet.KeyId);
            if(getKey != null)
            {
                _dbContext.DbCacheModels.Remove(getKey);
                _dbContext.DbCacheModels.Add(modelToSet);
            }
            else
            {
                _dbContext.DbCacheModels.Add(modelToSet);
            }
            return  _unitOfWork.SaveChangesAsync();
        }
    }
}
