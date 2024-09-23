using DiamondShop.Domain.Models.Diamonds.Entities;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondConfig
{
    internal class DiamondWarrantyConfiguration : IEntityTypeConfiguration<DiamondWarranty>
    {
        public void Configure(EntityTypeBuilder<DiamondWarranty> builder)
        {
            builder.ToTable("DiamondWarranty");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => DiamondId.Parse(dbValue));
            builder.HasKey(o => o.Id);
        }
    }
}
