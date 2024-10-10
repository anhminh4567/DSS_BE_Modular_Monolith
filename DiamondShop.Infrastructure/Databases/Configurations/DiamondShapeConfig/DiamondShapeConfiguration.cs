using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncfusion.XlsIO.Implementation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondShapeConfig
{
    internal class DiamondShapeConfiguration : IEntityTypeConfiguration<DiamondShape>
    {
        public static List<DiamondShape> SHAPES = new List<DiamondShape>
        {
            DiamondShape.Create("Round",DiamondShapeId.Parse(1.ToString())),
            DiamondShape.Create("Princess",DiamondShapeId.Parse(2.ToString())),
            DiamondShape.Create("Cushion",DiamondShapeId.Parse(3.ToString())),
            DiamondShape.Create("Emerald",DiamondShapeId.Parse(4.ToString())),
            DiamondShape.Create("Oval",DiamondShapeId.Parse(5.ToString())),
            DiamondShape.Create("Radiant",DiamondShapeId.Parse(6.ToString())),
            DiamondShape.Create("Asscher",DiamondShapeId.Parse(7.ToString())),
            DiamondShape.Create("Marquise",DiamondShapeId.Parse(8.ToString())),
            DiamondShape.Create("Heart",DiamondShapeId.Parse(9.ToString())),
            DiamondShape.Create("Pear",DiamondShapeId.Parse(10.ToString()))
        };
        public void Configure(EntityTypeBuilder<DiamondShape> builder)
        {
            builder.ToTable("Diamond_Shape");
            builder.Property(a => a.Id).HasConversion(a => a.Value, dbValue => DiamondShapeId.Parse(dbValue));
            builder.HasKey(a => a.Id);
            //builder.HasIndex(a => a.Id);
            builder.HasData(SHAPES);
        }
    }
}
