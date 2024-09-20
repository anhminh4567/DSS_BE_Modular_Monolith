using DiamondShop.Domain.Models.JewelryModels;
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
    internal class SideDiamondReqConfiguration : IEntityTypeConfiguration<SideDiamondReq>
    {
        public void Configure(EntityTypeBuilder<SideDiamondReq> builder)
        {
            builder.ToTable("SideDiamondReq");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => SideDiamondReqId.Parse(dbValue));
            builder.HasOne(o => o.DiamondShape).WithOne().HasForeignKey<SideDiamondReq>(p => p.DiamondShapeId).IsRequired();
            builder.Property(o => o.SettingType).HasConversion<string>().IsRequired();
            builder.Property(o => o.ColorMin).HasConversion<string>().IsRequired();
            builder.Property(o => o.ColorMax).HasConversion<string>().IsRequired();
            builder.Property(o => o.ClarityMin).HasConversion<string>().IsRequired();
            builder.Property(o => o.ClarityMax).HasConversion<string>().IsRequired();
            builder.HasKey(o => o.Id);
        }
    }
}
