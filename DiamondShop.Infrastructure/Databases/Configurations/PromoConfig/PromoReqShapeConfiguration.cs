using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.PromoConfig
{
    internal class PromoReqShapeConfiguration : IEntityTypeConfiguration<PromoReqShape>
    {
        public void Configure(EntityTypeBuilder<PromoReqShape> builder)
        {
            builder.ToTable("PromoReqShape");
            builder.Property(o => o.PromoReqId)
            .HasConversion(
                Id => Id.Value,
                dbValue => PromoReqId.Parse(dbValue));
            builder.Property(o => o.ShapeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondShapeId.Parse(dbValue));
            builder.HasOne(o => o.PromoReq).WithMany().HasForeignKey(o => o.PromoReqId).IsRequired();
            builder.HasOne(o => o.Shape).WithMany().HasForeignKey(o => o.ShapeId).IsRequired();
            builder.HasKey(o => new { o.PromoReqId, o.ShapeId});
        }
    }
}
