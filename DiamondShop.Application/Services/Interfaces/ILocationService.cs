using DiamondShop.Domain.Common.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface ILocationService
    {
        List<Province> GetProvinces();
        List<District> GetDistricts( string provinceId);
        List<Ward> GetWards(string districtId);


    }
}
