using DiamondShop.Domain.Models.DiamondPrices.Entities;
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
    internal class DiamondCriteriaConfiguration : IEntityTypeConfiguration<DiamondCriteria>
    {
        public void Configure(EntityTypeBuilder<DiamondCriteria> builder)
        {
            builder.ToTable("DiamondCriteria");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => DiamondCriteriaId.Parse(dbValue));
            builder.Property(o => o.ShapeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondShapeId.Parse(dbValue));
            builder.HasOne(o => o.Shape).WithMany().HasForeignKey(o => o.ShapeId).IsRequired();
            builder.HasIndex(x => new { x.CaratFrom,x.CaratTo,x.IsSideDiamond });
            builder.HasIndex(x => new { x.ShapeId, x.IsSideDiamond, x.CaratFrom, x.CaratTo });
            builder.HasKey(o => o.Id);
        }
    }
}
