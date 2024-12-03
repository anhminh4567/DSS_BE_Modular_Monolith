using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Addresses;
using DiamondShop.Domain.Models.Locations;
using DiamondShop.Domain.Repositories.LocationRepo;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Locations.OApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.LocationRepo
{
    internal class LocationRepository : ILocationRepository
    {
        private readonly DiamondShopDbContext _dbContext;
        private readonly OApiLocationService _oApiLocationService;

        public LocationRepository(DiamondShopDbContext dbContext , ILogger<OApiLocationService> logger1,IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _dbContext = dbContext;
            _oApiLocationService = new OApiLocationService(logger1, optionsMonitor);
        }

        public Task<List<Province>> GetAllProvince(CancellationToken cancellationToken = default)
        {
            return _dbContext.AppCities.Select(x => new Province() 
            {
                Id = x.Id.ToString(),
                IsActive = true,
                Name = x.Name,
                NameExtension = new string[] { x.Name , x.Slug }
            }).ToListAsync(cancellationToken);
        }

        public async Task<List<District>> GetDistricts(int provinceId, CancellationToken cancellationToken = default)
        {
            return _oApiLocationService.GetDistricts(provinceId.ToString());
            //return //_dbContext.AppDistricts.Where(x => x.ProvinceId == provinceId).ToListAsync(cancellationToken);
        }

        public async Task<Province?> GetProvince(int id, CancellationToken cancellationToken = default)
        {
            var result = _dbContext.AppCities.FirstOrDefault(x => x.Id == id);
            if(result == null)
            {
                return null;
            }
            return new Province()
            {
                Id = result.Id.ToString(),
                IsActive = true,
                Name = result.Name,
                NameExtension = new string[] { result.Name, result.Slug }
            };
        }

        public async Task<List<Ward>> GetWards(int districtId, CancellationToken cancellationToken = default)
        {
            return _oApiLocationService.GetWards(districtId.ToString());
            //return _dbContext.AppWards.Where(x => x.DistrictId == districtId).ToListAsync(cancellationToken);
        }
    }
}
