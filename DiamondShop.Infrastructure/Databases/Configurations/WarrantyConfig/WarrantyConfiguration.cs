using DiamondShop.Domain.Models.Warranties;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.WarrantyConfig
{
    internal class WarrantyConfiguration : IEntityTypeConfiguration<Warranty>
    {
        new public void Configure(EntityTypeBuilder<Warranty> builder)
        {
            builder.Property(o => o.Id)
                .HasConversion(
                 o => o.Value,
                 dbValue => WarrantyId.Parse(dbValue));
            builder.Property(o => o.Type).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }   
    }
}
