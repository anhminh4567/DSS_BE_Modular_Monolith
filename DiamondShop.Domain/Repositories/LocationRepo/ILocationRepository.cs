using DiamondShop.Domain.Common.Addresses;
using DiamondShop.Domain.Models.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.LocationRepo
{
    public interface ILocationRepository
    {
        Task<List<Province>> GetAllProvince(CancellationToken cancellationToken = default);
        Task<Province?> GetProvince(int id,CancellationToken cancellationToken = default);
        Task<List<District>> GetDistricts(int provinceId, CancellationToken cancellationToken = default);
        Task<List<Ward>> GetWards(int districtId, CancellationToken cancellationToken = default);
    }
}
