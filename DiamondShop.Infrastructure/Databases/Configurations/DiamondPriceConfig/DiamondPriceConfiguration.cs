using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondPriceConfig
{
    internal class DiamondPriceConfiguration : IEntityTypeConfiguration<DiamondPrice>
    {
        public void Configure(EntityTypeBuilder<DiamondPrice> builder)
        {
            builder.ToTable("DiamondPrice");
            builder.Property(o => o.ShapeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondShapeId.Parse(dbValue));
            builder.Property(o => o.CriteriaId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondCriteriaId.Parse(dbValue));
            builder.HasOne(o => o.Shape).WithMany().HasForeignKey(o => o.ShapeId).IsRequired();
            builder.HasOne(o => o.Criteria).WithMany().HasForeignKey(o => o.CriteriaId).IsRequired();
            builder.HasKey(o => new { o.ShapeId, o.CriteriaId,o.IsLabDiamond, o.IsSideDiamond });
            builder.HasIndex(o => new { o.ShapeId, o.IsLabDiamond, o.IsSideDiamond  });
            builder.HasIndex(o => new { o.CriteriaId, o.IsLabDiamond , o.IsSideDiamond });
            builder.HasIndex(o => new { o.CriteriaId, o.ShapeId, o.IsLabDiamond, o.IsSideDiamond });

        }
    }
}
