using DiamondShop.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Test.General
{
    public class TestDbContext : DiamondShopDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options,null) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase($"TestDatabase_{new Guid().ToString()}");
            }
        }
    }
}
