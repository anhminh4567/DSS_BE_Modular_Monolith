using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Configurations.OrderConfig
{
    internal class OrderItemDetailConfiguration : IEntityTypeConfiguration<OrderItemDetail>
    {
        public void Configure(EntityTypeBuilder<OrderItemDetail> builder)
        {
            builder.ToTable("OrderItemDetail");
            builder.Property(o => o.Id)
                .HasConversion(
                    o => o.Value,
                    dbValue => OrderItemDetailId.Parse(dbValue));
            builder.HasOne(o => o.Jewelry).WithOne().HasForeignKey<OrderItemDetail>(o => o.JewelryId).IsRequired(false);
            builder.HasOne(o => o.Diamond).WithOne().HasForeignKey<OrderItemDetail>(o => o.DiamondId).IsRequired(false);
            builder.HasOne(o => o.MainDetail).WithMany().HasForeignKey(o => o.MainDetailId).IsRequired(false);
            builder.HasKey(o => o.Id);

        }
    }
}
