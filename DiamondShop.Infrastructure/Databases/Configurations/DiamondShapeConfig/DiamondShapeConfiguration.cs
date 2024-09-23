using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondShapeConfig
{
    internal class DiamondShapeConfiguration : IEntityTypeConfiguration<DiamondShape>
    {
        public void Configure(EntityTypeBuilder<DiamondShape> builder)
        {
            builder.ToTable("Diamond_Shape");
            builder.Property(a => a.Id).HasConversion(a => a.Value, dbValue => DiamondShapeId.Parse(dbValue));
            builder.HasKey(a => a.Id);
            builder.HasIndex(a => a.Id);
        }
    }
}
