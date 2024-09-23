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
    internal class SideDiamondOptConfiguration : IEntityTypeConfiguration<SideDiamondOpt>
    {
        public void Configure(EntityTypeBuilder<SideDiamondOpt> builder)
        {
            builder.ToTable("SideDiamondOpt");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => SideDiamondOptId.Parse(dbValue));
            builder.Property(o => o.SideDiamondReqId)
            .HasConversion(
                Id => Id.Value,
                dbValue => SideDiamondReqId.Parse(dbValue));
            builder.HasKey(o => o.Id);
        }
    }
}
