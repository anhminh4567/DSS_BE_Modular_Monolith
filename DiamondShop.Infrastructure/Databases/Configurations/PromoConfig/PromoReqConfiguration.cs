using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.PromoConfig
{
    internal class PromoReqConfiguration : IEntityTypeConfiguration<PromoReq>
    {
        public void Configure(EntityTypeBuilder<PromoReq> builder)
        {
            builder.ToTable("PromoReq");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => PromoReqId.Parse(dbValue));
            builder.Property(o => o.PromotionId)
            .HasConversion(
                Id => Id.Value,
                dbValue => PromotionId.Parse(dbValue));
            builder.Property(o => o.DiscountId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiscountId.Parse(dbValue));
            builder.Property(o => o.ModelId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryModelId.Parse(dbValue));
            builder.HasOne(o => o.Discount).WithMany(o => o.DiscountReq).HasForeignKey(o => o.DiscountId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(o => o.Promotion).WithMany(o => o.PromoReqs).HasForeignKey(o => o.PromotionId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(o => o.Model).WithMany().HasForeignKey(o => o.ModelId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.TargetType).HasConversion<string>();
            builder.Property(o => o.Operator).HasConversion<string>();
            //builder.Property(o => o.DiamondOrigin).HasConversion<string>();
            //builder.Property(o => o.ClarityFrom).HasConversion<string>();
            //builder.Property(o => o.ClarityTo).HasConversion<string>();
            //builder.Property(o => o.CutFrom).HasConversion<string>();
            //builder.Property(o => o.CutTo).HasConversion<string>();
            //builder.Property(o => o.ColorFrom).HasConversion<string>();
            //builder.Property(o => o.ColorTo).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
