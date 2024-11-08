using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
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
            builder.OwnsOne(o => o.SideDiamond,
              sideDiamond =>
              {
                  sideDiamond.WithOwner();
                  sideDiamond.Property(n => n.Carat).HasColumnName("Carat");
                  sideDiamond.Property(n => n.Quantity).HasColumnName("Quantity");
                  sideDiamond.Property(n => n.SettingType).HasColumnName("SettingType").HasConversion<string>();
                  sideDiamond.Property(n => n.DiamondShapeId).HasColumnName("DiamondShapeId");
                  sideDiamond.HasOne(n => n.DiamondShape).WithOne().HasForeignKey<JewelrySideDiamond>(p => p.DiamondShapeId);
                  sideDiamond.Property(n => n.ColorMin).HasColumnName("ColorMin").HasConversion<string>();
                  sideDiamond.Property(n => n.ColorMax).HasColumnName("ColorMax").HasConversion<string>();
                  sideDiamond.Property(n => n.ClarityMin).HasColumnName("ClarityMin").HasConversion<string>();
                  sideDiamond.Property(n => n.ClarityMax).HasColumnName("ClarityMax").HasConversion<string>();
              });
            builder.HasMany(p => p.Diamonds).WithOne().HasForeignKey(p => p.JewelryId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            builder.HasOne(o => o.Model).WithMany().HasForeignKey(o => o.ModelId);
            builder.HasOne(o => o.Size).WithMany().HasForeignKey(o => o.SizeId);
            builder.HasOne(o => o.Metal).WithMany().HasForeignKey(o => o.MetalId);
            builder.HasOne(o => o.Review).WithOne(p => p.Jewelry).HasForeignKey<JewelryReview>(o => o.Id).IsRequired(false);
            builder.OwnsOne(o => o.Thumbnail, childBuilder =>
            {
                childBuilder.ToJson();
            });
            builder.OwnsOne(o => o.ProductLock, childBuilder =>
            {
                childBuilder.Property(o => o.AccountId)
                .HasConversion(
                    Id => Id.Value,
                    dbValue => AccountId.Parse(dbValue));
            });
            builder.HasKey(o => o.Id);
            //builder.HasIndex(o => o.Id);
        }
    }
}
