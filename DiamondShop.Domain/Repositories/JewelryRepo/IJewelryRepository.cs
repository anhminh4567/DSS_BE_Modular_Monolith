using DiamondShop.Domain.Models.Jewelries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.JewelryRepo
{
    public interface IJewelryRepository : IBaseRepository<Jewelry>
    {
        public Task<(List<Jewelry> jewelries, int totalPage)> GetSellingJewelry(int skip, int take);
        public Task<bool> CheckDuplicatedSerial(string serialNumber);
    }
}
