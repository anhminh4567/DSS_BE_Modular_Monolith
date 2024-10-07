using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.PromoConfig
{
    internal class GiftConfiguration : IEntityTypeConfiguration<Gift>
    {
        public void Configure(EntityTypeBuilder<Gift> builder)
        {
            builder.ToTable("Gift");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => GiftId.Parse(dbValue));
            builder.Property(o => o.PromotionId)
            .HasConversion(
                Id => Id.Value,
                dbValue => PromotionId.Parse(dbValue));
            builder.Property(o => o.TargetType).HasConversion<string>();
            builder.Property(o => o.UnitType).HasConversion<string>();
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TargetType).HasConversion<string>();

            //builder.Property(o => o.DiamondOrigin).HasConversion<string>();
            //builder.Property(o => o.ClarityFrom).HasConversion<string>();
            //builder.Property(o => o.ClarityTo).HasConversion<string>();
            //builder.Property(o => o.CutFrom).HasConversion<string>();
            //builder.Property(o => o.CutTo).HasConversion<string>();
            //builder.Property(o => o.ColorFrom).HasConversion<string>();
            //builder.Property(o => o.ColorTo).HasConversion<string>();

            builder.OwnsMany(o => o.DiamondGiftShapes,owndedBuilder =>
            {
                owndedBuilder.ToJson();
            });
        }
    }
}
