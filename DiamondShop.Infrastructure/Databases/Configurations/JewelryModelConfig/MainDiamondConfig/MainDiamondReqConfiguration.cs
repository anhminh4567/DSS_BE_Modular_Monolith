using DiamondShop.Domain.Models.JewelryModels.Entities;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryModelConfig.MainDiamondConfig
{
    internal class MainDiamondReqConfiguration : IEntityTypeConfiguration<MainDiamondReq>
    {
        public void Configure(EntityTypeBuilder<MainDiamondReq> builder)
        {
            builder.ToTable("MainDiamondReq");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => MainDiamondReqId.Parse(dbValue));
            builder.Property(o => o.ModelId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryModelId.Parse(dbValue));
            builder.HasMany(o => o.Shapes).WithOne(p => p.MainDiamondReq).HasForeignKey(p => p.MainDiamondReqId).IsRequired();
            builder.Property(o => o.SettingType).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
