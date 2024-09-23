using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
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
    internal class MainDiamondShapeConfiguration : IEntityTypeConfiguration<MainDiamondShape>
    {
        public void Configure(EntityTypeBuilder<MainDiamondShape> builder)
        {
            builder.ToTable("MainDiamondShape");
            builder.Property(o => o.MainDiamondReqId)
            .HasConversion(
                Id => Id.Value,
                dbValue => MainDiamondReqId.Parse(dbValue));
            builder.Property(o => o.ShapeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondShapeId.Parse(dbValue));
            builder.HasOne(o => o.Shape).WithOne().HasForeignKey<MainDiamondShape>(o => o.ShapeId).IsRequired();
            builder.HasOne(o => o.MainDiamondReq).WithOne().HasForeignKey<MainDiamondShape>(o => o.MainDiamondReqId).IsRequired();
            builder.HasKey(o => new { o.MainDiamondReqId, o.ShapeId });
        }
    }
}
