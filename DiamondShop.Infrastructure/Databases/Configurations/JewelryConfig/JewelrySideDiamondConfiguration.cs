//using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
//using DiamondShop.Domain.Models.Jewelries.Entities;
//using DiamondShop.Domain.Models.Jewelries.ValueObjects;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryConfig
//{
//    internal class JewelrySideDiamondConfiguration : IEntityTypeConfiguration<JewelrySideDiamond>
//    {
//        public void Configure(EntityTypeBuilder<JewelrySideDiamond> builder)
//        {
//            builder.ToTable("JewelrySideDiamond");
//            builder.Property(o => o.Id)
//                .HasConversion(
//                o => o.Value,
//                dbValue => JewelrySideDiamondId.Parse(dbValue));
//            builder.Property(o => o.JewelryId)
//            .HasConversion(
//                Id => Id.Value,
//                dbValue => JewelryId.Parse(dbValue));
//            builder.Property(o => o.DiamondShapeId)
//            .HasConversion(
//                Id => Id.Value,
//                dbValue => DiamondShapeId.Parse(dbValue));
//            builder.HasOne(d => d.DiamondShape)
//                .WithMany()
//                .HasForeignKey(d => d.DiamondShapeId)
//                .IsRequired(false);
//            //builder.Property(o => o.ColorMin).HasConversion<string>();
//            //builder.Property(o => o.ColorMax).HasConversion<string>();
//            //builder.Property(o => o.ClarityMin).HasConversion<string>();
//            //builder.Property(o => o.ColorMax).HasConversion<string>();
//            builder.Property(o => o.SettingType).HasConversion<string>();
//            builder.HasKey(o => o.Id);
//            //builder.HasIndex(o => o.Id);
//        }
//    }
//}
