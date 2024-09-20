using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryConfig
{
    internal class JewelryWarrantyConfiguration : IEntityTypeConfiguration<JewelryWarranty>
    {
        public void Configure(EntityTypeBuilder<JewelryWarranty> builder)
        {
            builder.ToTable("JewelryWarranty");
            builder.Property(o => o.Id)
                .HasConversion(
                o => o.Value,
                dbValue => JewelryId.Parse(dbValue));
            builder.Property(o => o.Status).HasConversion<string>();
            builder.Property(o => o.Type).HasConversion<string>();
            builder.HasKey(o => o.Id);
            builder.HasIndex(o => o.Id);
        }
    }
}
