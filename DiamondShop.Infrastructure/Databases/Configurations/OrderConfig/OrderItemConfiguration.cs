using DiamondShop.Domain.Models.Orders.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Orders.ValueObjects;

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
            builder.HasMany(o => o.Details).WithOne().HasForeignKey(p => p.ItemId).IsRequired();
            builder.Property(o => o.Status).HasConversion<string>();
            builder.HasKey(o => o.Id);
        }
    }
}
