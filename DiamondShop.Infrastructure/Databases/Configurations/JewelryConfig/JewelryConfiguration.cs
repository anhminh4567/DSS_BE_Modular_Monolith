using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.JewelryConfig
{
    internal class JewelryConfiguration : IEntityTypeConfiguration<Jewelry>
    {
        public void Configure(EntityTypeBuilder<Jewelry> builder)
        {
            builder.ToTable("Jewelry");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => JewelryId.Parse(dbValue));
            builder.Property(o => o.ModelId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryModelId.Parse(dbValue));
            builder.Property(o => o.SizeId)
            .HasConversion(
                Id => Id.Value,
                dbValue => SizeId.Parse(dbValue));
            builder.Property(o => o.MetalId)
            .HasConversion(
                Id => Id.Value,
                dbValue => MetalId.Parse(dbValue));
            builder.Property(o => o.ReviewId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryReviewId.Parse(dbValue));
            builder.HasMany(o => o.Diamonds).WithOne().HasForeignKey(p => p.JewelryId);
            builder.HasOne(o => o.Model).WithOne().HasForeignKey<Jewelry>(o => o.ModelId);
            builder.HasOne(o => o.Size).WithOne().HasForeignKey<Jewelry>(o => o.SizeId);
            builder.HasOne(o => o.Metal).WithOne().HasForeignKey<Jewelry>(o => o.MetalId);
            builder.HasOne(o => o.Review).WithOne().HasForeignKey<JewelryReview>(o => o.Id).IsRequired(false);
            //builder.HasOne(o => o.Warranty).WithOne().HasForeignKey<JewelryWarranty>(o => o.Id).IsRequired(false);
            builder.HasMany(o => o.SideDiamonds).WithOne().HasForeignKey(o => o.JewelryId).IsRequired(false);
            builder.OwnsOne(o => o.Thumbnail, childBuilder =>
            {
                childBuilder.ToJson();
            });
            builder.HasKey(o => o.Id);
            builder.HasIndex(o => o.Id);
        }
    }
}
