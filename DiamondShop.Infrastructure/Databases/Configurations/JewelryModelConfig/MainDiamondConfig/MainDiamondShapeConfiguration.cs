using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryModelConfig.MainDiamondConfig
{
    internal class MainDiamondShapeConfiguration : IEntityTypeConfiguration<MainDiamondShape>
    {
        public void Configure(EntityTypeBuilder<MainDiamondShape> builder)
        {
            builder.ToTable("MainDiamondShape");
            builder.HasOne(o => o.DiamondShape).WithOne().HasForeignKey<MainDiamondShape>(o => o.DiamondShapeId).IsRequired();
            builder.HasOne(o => o.MainDiamond).WithOne().HasForeignKey<MainDiamondShape>(o => o.MainDiamondId).IsRequired();
            builder.HasKey(o => new { o.MainDiamondId, o.DiamondShapeId });
        }
    }
}
