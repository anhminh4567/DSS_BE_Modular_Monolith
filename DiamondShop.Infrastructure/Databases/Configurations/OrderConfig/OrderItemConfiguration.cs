using DiamondShop.Domain.Models.Orders.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;

namespace DiamondShop.Infrastructure.Databases.Configurations.OrderConfig
{
    internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => OrderItemId.Parse(dbValue));
            builder.Property(o => o.JewelryId)
            .HasConversion(
                Id => Id.Value,
                dbValue => JewelryId.Parse(dbValue));
            builder.Property(o => o.DiamondId)
            .HasConversion(
                Id => Id.Value,
                dbValue => DiamondId.Parse(dbValue));
            builder.Property(o => o.Status).HasConversion<string>();
            builder.HasOne(o => o.Discount).WithMany().HasForeignKey(o => o.DiscountId).IsRequired(false);
            builder.HasOne(o => o.Jewelry).WithOne().HasForeignKey<OrderItem>(o => o.JewelryId).IsRequired(false);
            builder.HasOne(o => o.Diamond).WithOne().HasForeignKey<OrderItem>(o => o.DiamondId).IsRequired(false);
            builder.HasKey(o => o.Id);
        }
    }
}
