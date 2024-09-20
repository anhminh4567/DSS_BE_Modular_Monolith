using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryModelConfig.SideDiamondConfig
{
    internal class SideDiamondConfiguration : IEntityTypeConfiguration<SideDiamond>
    {
        public void Configure(EntityTypeBuilder<SideDiamond> builder)
        {
            builder.ToTable("SideDiamond");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => SideDiamondId.Parse(dbValue));
            builder.HasMany(o => o.SideDiamondReqs).WithOne().HasForeignKey(p => p.SideDiamondId).IsRequired();
            builder.HasKey(o => o.Id);
        }
    }
}
