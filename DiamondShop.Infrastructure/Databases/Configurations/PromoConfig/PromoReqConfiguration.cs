using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;

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
            builder.HasOne(o => o.Discount).WithMany().HasForeignKey(o => o.DiscountId).IsRequired();
            builder.HasOne(o => o.JewelryModel).WithMany().HasForeignKey(o => o.JewelryModelId).IsRequired(false);
            builder.Property(o => o.TargetType).HasConversion<string>();
            builder.Property(o => o.Operator).HasConversion<string>();
            builder.Property(o => o.DiamondOrigin).HasConversion<string>();
            builder.Property(o => o.ClarityFrom).HasConversion<string>();
            builder.Property(o => o.ClarityTo).HasConversion<string>();
            builder.Property(o => o.CutFrom).HasConversion<string>();
            builder.Property(o => o.CutTo).HasConversion<string>();
            builder.Property(o => o.ColorFrom).HasConversion<string>();
            builder.Property(o => o.ColorTo).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
