using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.DiamondConfig
{
    internal class DiamondConfiguration : IEntityTypeConfiguration<Diamond>
    {
        public void Configure(EntityTypeBuilder<Diamond> builder)
        {
            builder.ToTable("Diamond");
            builder.Property(o => o.Id)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => DiamondId.Parse(dbValue));
            builder.Property(o => o.DiamondShapeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondShapeId.Parse(dbValue));
            builder.HasOne(o => o.DiamondShape)
                .WithMany()
                .HasForeignKey(c => c.DiamondShapeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.SetNull);
            //builder.HasOne(o => o.Warranty).WithOne().HasForeignKey<DiamondWarranty>(p => p.Id).IsRequired(false);
            /*builder.HasMany(o => o.Medias).WithOne().HasForeignKey(p => p.DiamondId);*/
            builder.OwnsOne(o => o.Thumbnail, childBuilder =>
            {
                childBuilder.ToJson();
            });
            builder.OwnsOne(o => o.CertificateFilePath, childBuilder =>
            {
                
            });
            builder.OwnsOne(o => o.ProductLock, childBuilder =>
            {
                childBuilder.Property(o => o.AccountId)
                .HasConversion( 
                    Id => Id.Value,
                    dbValue => AccountId.Parse(dbValue));
            });
            builder.Property(o => o.JewelryId)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => JewelryId.Parse(dbValue)).IsRequired(false);
            builder.HasKey(o => o.Id);
            builder.HasIndex(o => new { o.Carat, o.Color, o.Clarity, o.Cut, o.IsLabDiamond, o.JewelryId });
        }
    }
}
