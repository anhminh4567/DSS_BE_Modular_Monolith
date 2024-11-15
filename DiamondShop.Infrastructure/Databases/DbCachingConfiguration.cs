using DiamondShop.Application.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases
{
    internal class DbCachingConfiguration : IEntityTypeConfiguration<DbCacheModel>
    {
        public void Configure(EntityTypeBuilder<DbCacheModel> builder)
        {
            builder.HasKey(x => x.KeyId);
        }
    }
}
