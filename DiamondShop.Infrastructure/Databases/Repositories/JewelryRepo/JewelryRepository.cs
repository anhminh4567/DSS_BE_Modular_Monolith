using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Repositories.JewelryRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.JewelryRepo
{
    internal class JewelryRepository : BaseRepository<Jewelry>, IJewelryRepository
    {
        public JewelryRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {

        }
        public async Task<bool> CheckDuplicatedSerial(string serialNumber)
        {
            return await _set.AnyAsync(p => p.SerialCode == serialNumber);
        }

    }
}
