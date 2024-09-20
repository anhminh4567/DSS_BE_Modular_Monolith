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
    internal class MainDiamondConfiguration : IEntityTypeConfiguration<MainDiamond>
    {
        public void Configure(EntityTypeBuilder<MainDiamond> builder)
        {
            builder.ToTable("MainDiamond");
            builder.Property(o => o.Id)
               .HasConversion(
                   Id => Id.Value,
                   dbValue => MainDiamondId.Parse(dbValue));
            builder.HasMany(o => o.DiamondShapes).WithOne().HasForeignKey(p => p.MainDiamondId).IsRequired();
            builder.Property(o => o.SettingType).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
